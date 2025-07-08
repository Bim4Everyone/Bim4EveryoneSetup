using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace BIM4EveryoneSetup;

/// <summary>
///     https://github.com/nuke-build/nuke/blob/develop/source/Nuke.Tooling/ProcessTasks.cs
/// </summary>
internal sealed class Process2 {
    public static IEnumerable<string> StartProcess(
        string toolPath,
        string? arguments = null,
        string? workingDirectory = null) {
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

        Console.WriteLine($"[INF] > {startInfo.FileName} {startInfo.Arguments}");

        Process? process = Process.Start(startInfo);
        if(process == null) {
            return [];
        }

        var output = new BlockingCollection<string>();

        process.OutputDataReceived += (_, e) => {
            if(e.Data == null) {
                return;
            }

            output.Add(e.Data);
            Console.WriteLine("[INF] " + e.Data);
        };

        process.ErrorDataReceived += (_, e) => {
            if(e.Data == null) {
                return;
            }

            output.Add(e.Data);
            Console.WriteLine("[ERR] " + e.Data);
        };

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.WaitForExit();
        return output;
    }
}
