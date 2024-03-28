using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BIM4EveryoneSetup {
    /// <summary>
    /// https://github.com/nuke-build/nuke/blob/develop/source/Nuke.Tooling/ProcessTasks.cs
    /// </summary>
    internal sealed class Process2 {
        public static IEnumerable<string> StartProcess(
            string toolPath,
            string arguments = default,
            string workingDirectory = default) {

            var startInfo = new ProcessStartInfo {
                FileName = toolPath,
                Arguments = arguments ?? string.Empty,
                WorkingDirectory = workingDirectory ?? string.Empty,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8
            };

            Process process = Process.Start(startInfo);
            if(process == default) {
                return Enumerable.Empty<string>();
            }

            var output = new BlockingCollection<string>();
           
            process.OutputDataReceived += (_, e) => {
                if(e.Data == null)
                    return;

                output.Add(e.Data);
                Console.WriteLine("INFO: " + e.Data);
            };

            process.ErrorDataReceived += (_, e) => {
                if(e.Data == null)
                    return;

                output.Add(e.Data);
                Console.WriteLine("ERROR: " + e.Data);
            };

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
            return output;
        }
    }
}