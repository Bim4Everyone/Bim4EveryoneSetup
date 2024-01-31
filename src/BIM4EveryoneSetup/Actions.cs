using System;
using System.Diagnostics;
using System.IO;

using WixSharp;

using WixToolset.Dtf.WindowsInstaller;

using File = System.IO.File;

namespace BIM4EveryoneSetup {
    public static class Actions {
        [CustomAction]
        public static ActionResult UpdateProperties(Session session) {
            session.Log("Started update properties");

            if(File.Exists(Constants.UninstallerFile)) {
                var productVersion =
                    FileVersionInfo.GetVersionInfo(Constants.UninstallerFile).ProductVersion.Trim();
                session[Constants.pyRevitVersionProp] = productVersion;
            }

            var isInstalled = Directory.Exists(Constants.pyRevitPath);
            session[Constants.pyRevitInstalledProp] = isInstalled.ToString();

            session.Log("Finished update properties");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult Uninstall(Session session) {
            session.Log("Started uninstall pyRevit");

            string uninstallFile = Constants.UninstallerFile;
            if(File.Exists(uninstallFile)) {
                Process.Start(uninstallFile, "/VERYSILENT")?.WaitForExit();
                session.Log($"Uninstalled pyRevit: {uninstallFile}");
            }

            string pyRevitPath = Constants.pyRevitPath;
            if(Directory.Exists(pyRevitPath)) {
                new DirectoryInfo(pyRevitPath).RemoveDirectory();
                session.Log($"Removed pyRevit directory: \"{pyRevitPath}\"");
            }

            session.Log("Finished uninstall pyRevit");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult UninstallBundles(Session session) {
            session.Log("Started uninstall bundles");

            string pyRevitBundlesPath = Constants.BundlesPath;
            if(Directory.Exists(pyRevitBundlesPath)) {
                new DirectoryInfo(pyRevitBundlesPath).RemoveDirectory();
                session.Log($"Removed pyRevit bundles directory: \"{pyRevitBundlesPath}\"");
            }
            
            string pluginSettings = Constants.PluginSettingsPath;
            if(Directory.Exists(pluginSettings)) {
                new DirectoryInfo(pluginSettings).RemoveDirectory();
                session.Log($"Removed plugin settings directory: \"{pyRevitBundlesPath}\"");
            }
            
            session.Log("Finished uninstall bundles");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult Configure(Session session) {
            session.Log("Started configure pyRevit");

            string batFile = Constants.ConfigureFile;
            try {
                session.SaveBinary(Constants.ConfigureFileProp, batFile);
                session.Log($"Saved {Constants.ConfigureFileProp}: \"{batFile}\"");

                var startInfo = new ProcessStartInfo(batFile) {CreateNoWindow = true, UseShellExecute = false};
                Process.Start(startInfo)?.WaitForExit();
                session.Log($"Started {Constants.ConfigureFileProp}: \"{batFile}\"");
            } finally {
                try {
                    File.Delete(batFile);
                    session.Log($"Removed {Constants.ConfigureFileProp}: \"{batFile}\"");
                } catch {
                    // do nothing
                }
            }

            session.Log("Finished configure pyRevit");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ModifyExtensions(Session session) {
            if(!session.IsModifying()) {
                session.Log("Skipped remove is not modify");
                return ActionResult.NotExecuted;
            }
            
            if(string.IsNullOrEmpty(session["REMOVE"])) {
                session.Log("Skipped \"REMOVE\" is empty");
                return ActionResult.NotExecuted;
            }

            session.Log($"Started modify extensions");
            string extensionsFile = Constants.ExtensionsFile;
            try {
                var removeFeatures = session["REMOVE"];
                session.Log($"Remove features: \"{removeFeatures}\"");

                session.SaveBinary(Constants.ExtensionsFileProp, extensionsFile);
                session.Log($"Saved {Constants.ExtensionsFileProp}: \"{extensionsFile}\"");

                string fileContent = File.ReadAllText(extensionsFile);
                foreach(FeatureExtension featureExtension in FeatureExtension.GetFeatures(fileContent)) {
                    if(removeFeatures.Contains(featureExtension.Id)) {
                        new DirectoryInfo(featureExtension.TargetPath).RemoveDirectory();
                        session.Log(
                            $"Removed feature \"{featureExtension.Name}\" directory: \"{featureExtension.TargetPath}\"");
                    }
                }
            } catch(Exception ex) {
                session.Log("Ошибка изменений расширений:");
                session.Log(ex.ToString());
                return ActionResult.NotExecuted;
            } finally {
                try {
                    File.Delete(extensionsFile);
                    session.Log($"Removed {Constants.ExtensionsFileProp}: \"{extensionsFile}\"");
                } catch {
                    // do nothing
                }
            }

            session.Log($"Finished modify extensions");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RepairExtensions(Session session) {
            if(!session.IsRepairing()) {
                session.Log("Skipped is not repairing");
                return ActionResult.NotExecuted;
            }

            if(string.IsNullOrEmpty(session["REINSTALL"])) {
                session.Log("Skipped \"REINSTALL\" is empty");
                return ActionResult.NotExecuted;
            }

            session.Log($"Started repair extensions");
            string extensionsFile = Constants.ExtensionsFileProp;
            try {
                var reinstallFeatures = session["REINSTALL"];
                session.Log($"Reinstall features: \"{reinstallFeatures}\"");
                
                session.SaveBinary(Constants.ExtensionsFileProp, extensionsFile);
                session.Log($"Saved {Constants.ExtensionsFileProp}: \"{extensionsFile}\"");

                string fileContent = File.ReadAllText(extensionsFile);
                foreach(FeatureExtension featureExtension in FeatureExtension.GetFeatures(fileContent)) {
                    new DirectoryInfo(featureExtension.TargetPath).RemoveDirectory();
                    session.Log(
                        $"Removed feature \"{featureExtension.Name}\" directory: \"{featureExtension.TargetPath}\"");
                }
            } catch(Exception ex) {
                session.Log(ex.ToString());
                return ActionResult.NotExecuted;
            } finally {
                try {
                    File.Delete(extensionsFile);
                    session.Log($"Removed {Constants.ExtensionsFileProp}: \"{extensionsFile}\"");
                } catch {
                    // do nothing
                }
            }

            session.Log($"Finished repair extensions");
            return ActionResult.Success;
        }
    }
}