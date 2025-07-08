using System.IO;
using System.Text;

using Newtonsoft.Json.Linq;

using WixSharp;

using File = System.IO.File;

namespace BIM4EveryoneSetup;

internal sealed class FeatureExtension {
    public string Id => $"_{Name?.Replace('-', '_')}_";
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string RepositoryUrl { get; private set; }

    public bool Enabled { get; private set; }
    public bool AllowChange { get; private set; }

    public string SourcePath => Path.Combine(Constants.BinPath, Name);
    public string TargetPath => Path.Combine(Constants.pyRevitExtensionsPath, Name);

    public string SourceFullPath => Path.GetFullPath(SourcePath);

    public void GitClone() {
        Process2.StartProcess("git", $"clone {RepositoryUrl} {SourceFullPath}");
    }

    public void UpdateRemote(string token) {
        Process2.StartProcess("git",
            $"remote set-url origin {CreateRepoUrlWithToken(token)}",
            SourceFullPath);
    }

    public void CreateTag(string tag) {
        Process2.StartProcess("git",
            $"tag {tag}",
            SourceFullPath);
    }

    public void PushTag(string tag) {
        Process2.StartProcess("git",
            $"push origin {tag}",
            SourceFullPath);
    }

    public void GetChanges(string tag, string lastTag, StringBuilder stringBuilder) {
        string? changes = Extensions.GetChanges(RepositoryUrl, SourceFullPath);
        if(string.IsNullOrEmpty(changes)) {
            return;
        }

        stringBuilder.AppendLine($"[{Name}]({RepositoryUrl}/compare/{lastTag}...{tag})");
        stringBuilder.AppendLine(changes);
        stringBuilder.AppendLine();
    }

    public Dir CreateDir() {
        Feature feature = CreateFeature();
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
            ?.Select(item => new FeatureExtension {
                Name = item.Value<string>("name") + "." + item.Value<string>("type"),
                RepositoryUrl = item.Value<string>("url")?.Replace(".git", string.Empty)!,
                Description = item.Value<string>("description")!,
                Enabled = item.Value<bool>("builtin"),
                AllowChange = !item.Value<bool>("builtin")
            }) ?? [];
    }

    private string CreateRepoUrlWithToken(string token) {
        return new Uri(new UriBuilder(RepositoryUrl) {UserName = token}.ToString()).ToString();
    }
}
