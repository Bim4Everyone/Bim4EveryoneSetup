using System;
using System.IO;
using System.Linq;

using WixSharp;

namespace BIM4EveryoneSetup {
    internal sealed class Constants {
        public static readonly Guid ProductId = new Guid("180C9E43-A015-428E-8FCC-34EDDBBAE855");

        public static readonly string ProductName = "Bim4Everyone";

        public static readonly string ProductDescription =
            "Платформа разрабатываемая для упрощения разработки проектной документации в Autodesk Revit.";

        public static readonly string ProductUrl = "https://github.com/Bim4Everyone/Bim4EveryoneSetup";
        public static readonly string ProductHelpUrl = "https://github.com/Bim4Everyone";
        public static readonly string ProductAboutUrl = "https://github.com/Bim4Everyone";
        public static readonly string ProductContacts = "https://github.com/Bim4Everyone";

        public static readonly string BinPath = "../../bin";
        public static readonly string ChangelogFile = "../../CHANGELOG.md";
        public static readonly string ReleaseChangelogFile = "../../bin/CHANGELOG.md";

        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitVersion = "4.8.16.24121";

        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitInstallUrl =
            $@"https://github.com/pyrevitlabs/pyRevit/releases/download/v{pyRevitVersion}+2117/pyRevit_{pyRevitVersion}_signed.exe";

        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitInstallFile =
            Path.Combine(BinPath, $"pyRevit_{pyRevitVersion}_signed.exe");

        public static readonly string MsiVersionFile = Path.Combine(BinPath, "msi_version.txt");
        public static readonly string CurrentDate = DateTime.Now.ToString("yy.MM.dd");
        public static readonly string CurrentTag = $"v{CurrentDate}";
        public static readonly Version CurrentVersion = new Version(CurrentDate);

        public static readonly string
            MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public static readonly string
            AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static readonly string PluginSettingsPath = Path.Combine(MyDocuments, "dosymep");

        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitPath = Path.Combine(AppDataPath, "pyRevit-Master");
        public static readonly string UninstallerFile = Path.Combine(pyRevitPath, "unins000.exe");

        // нужно для костылей
        public static readonly string InvokeDllPath = Path.Combine(pyRevitPath, @"pyrevitlib\pyrevit\runtime");
        public static readonly string CoreExtensionPath = Path.Combine(pyRevitPath, @"extensions\pyRevitCore.extension");

        public static readonly string AssetsPath = @"..\..\assets";
        public static readonly string BundlesPath = Path.Combine(AppDataPath, "pyRevit");

        public static readonly string ExtensionsFileProp = "_extensions.json_";
        public static readonly string ExtensionsFile = Path.Combine(BundlesPath, "extensions.json");
        public static readonly string ExtensionsAssetFile = Path.Combine(BinPath, "extensions.json");

        public static readonly string RevitIconAssetFile = Path.Combine(AssetsPath, "revit.png");
        public static readonly string ProductLicenceAssetFile = Path.Combine(AssetsPath, "license.rtf");
        public static readonly string ProductIconAssetFile = Path.Combine(AssetsPath, "Bim4Everyone_transparent.ico");
        public static readonly string WixUIBannerBmp = Path.Combine(AssetsPath, "WixUIBannerBmp.png");
        public static readonly string WixUIDialogBmp = Path.Combine(AssetsPath, "WixUIDialogBmp.png");

        public static readonly string ExtensionsFileUrl =
            @"https://raw.githubusercontent.com/Bim4Everyone/BIMExtensions/master/extensions.json";

        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitExtensionsPath = Path.Combine(AppDataPath, @"pyRevit\Extensions");

        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitCliPath = $@"%AppData%\pyRevit-master\bin\pyrevit.exe";

        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitExtensionsDirPath = $@"%AppData%\pyRevit\Extensions";

        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitVersionProp = "pyRevitVersion";

        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitInstalledProp = "pyRevitInstalled";

        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitInstallFileProp = "pyRevitInstallFile";

        public static readonly Condition InstallCondition = Condition.NOT_Installed;
        public static readonly Condition ChangeCondition = Condition.Create(" (REMOVE) ");
        public static readonly Condition RepairCondition = Condition.Create(" (REINSTALL) ");
        public static readonly Condition RemoveCondition = Condition.BeingUninstalled;
        public static readonly Condition ConfigInstallCondition = Constants.RepairCondition | Constants.InstallCondition;
       
        // ReSharper disable once InconsistentNaming
        public static readonly Condition pyRevitInstallCondition = Condition.Create($"{pyRevitVersionProp} < \"{pyRevitVersion}\"");


        public static string LastTag => Process2.StartProcess("git", "tag --sort=-creatordate").First();
    }
}