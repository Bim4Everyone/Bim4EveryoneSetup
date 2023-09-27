using System.IO;
using System.Net;

namespace BIM4EveryoneSetup {
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
            
            using(var client = new WebClient()) {
                client.DownloadFile(address, fileName);
            }
        }
    }
}