using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace BIM4EveryoneSetup;

internal static class Extensions {
    public static void RemoveDirectory(this DirectoryInfo directoryInfo) {
        if(!directoryInfo.Exists) {
            return;
        }

        directoryInfo.Attributes = FileAttributes.Normal;

        FileSystemInfo[] entities = directoryInfo.GetFileSystemInfos("*", SearchOption.AllDirectories);
        foreach(FileSystemInfo info in entities) {
            info.Attributes = FileAttributes.Normal;
        }

        directoryInfo.Delete(true);
    }

    public static void DownloadFile(string address, string fileName) {
        if(File.Exists(fileName)) {
            return;
        }

        using var client = new WebClient();
        client.DownloadFile(address, fileName);
    }

    public static string? GetChanges(string repoUrl, string? workingDir) {
        string[] list = Process2.StartProcess(
                "git",
                $"log --pretty=format:%s {Constants.LastTag}..HEAD",
                workingDir)
            .Where(item => !string.IsNullOrEmpty(item))
            .Select(item => Regex.Replace(item, @"#\d+", $@"[$0]({repoUrl}/pull/$0)"))
            .Select(item => item.Replace("/pull/#", "/pull/"))
            .ToArray();

        return list.Length == 0
            ? null
            : Environment.NewLine + " - " + string.Join(Environment.NewLine + " - ", list).Trim();
    }

    public static void InsertText(string filename, string text) {
        if(!File.Exists(filename)) {
            File.WriteAllText(filename, text);
            return;
        }

        string tempFile = Path.GetTempFileName();
        using(var writer = new StreamWriter(tempFile))
        using(var reader = new StreamReader(filename)) {
            writer.WriteLine(text);
            while(!reader.EndOfStream) {
                writer.WriteLine(reader.ReadLine());
            }
        }

        File.Copy(tempFile, filename, true);
        File.Delete(tempFile);
    }
}
