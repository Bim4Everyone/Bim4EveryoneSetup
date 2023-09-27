using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

using Newtonsoft.Json.Linq;

using WixSharp;

using File = System.IO.File;

namespace BIM4EveryoneSetup {
    internal sealed class FeatureExtension {
        public string Id => $"_{Name.Replace('-', '_')}_";
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string RepositoryName { get; private set; }

        public bool Enabled { get; private set; }
        public bool AllowChange { get; private set; }
        public string SourcePath => Path.Combine(Constants.BinPath, Name);
        public string TargetPath => Path.Combine(Constants.pyRevitExtensionsPath, Name);

        public void GitClone() {
            var processStartInfo = new ProcessStartInfo() {
                FileName = "git",
                Arguments = $"clone {RepositoryName} {Path.GetFullPath(SourcePath)}",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };

            using(Process process = Process.Start(processStartInfo)) {
                process.EnableRaisingEvents = true;
                process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);
                process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);

                process.BeginOutputReadLine();
                process.WaitForExit();
            }
        }

        public Dir CreateDir() {
            var feature = CreateFeature();
            return new Dir(feature, Name, new Files(feature, Path.Combine(SourcePath, "*.*")));
        }

        public Feature CreateFeature() {
            return new Feature(Name, Enabled, AllowChange) {Id = Id, Description = Description};
        }

        public static IEnumerable<FeatureExtension> GetFeatures() {
            return GetFeatures(File.ReadAllText(Constants.ExtensionsAssetFile));
        }

        public static IEnumerable<FeatureExtension> GetFeatures(string fileContent) {
            return JObject.Parse(fileContent)
                .GetValue("extensions")
                ?.ToObject<JToken[]>()
                .Select(item => new FeatureExtension() {
                    Name = item.Value<string>("name") + "." + item.Value<string>("type"),
                    RepositoryName = item.Value<string>("url"),
                    Description = item.Value<string>("description"),
                    Enabled = item.Value<bool>("builtin"),
                    AllowChange = !item.Value<bool>("builtin")
                }) ?? Enumerable.Empty<FeatureExtension>();
        }
    }
}