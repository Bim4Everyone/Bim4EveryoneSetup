using System;
using System.IO;

using WixSharp;

namespace BIM4EveryoneSetup {
    internal sealed class Constants {
        public static readonly Guid ProductId = new Guid("180C9E43-A015-428E-8FCC-34EDDBBAE855");

        public static readonly string ProductName = "Bim4Everyone";

        public static readonly string ProductDescription =
            "Платформа разрабатываемая для упрощения разработки проектной документации в Autodesk Revit.";

        public static readonly string ProductHelpUrl = "https://github.com/Bim4Everyone";
        public static readonly string ProductAboutUrl = "https://github.com/Bim4Everyone";
        public static readonly string ProductContacts = "https://github.com/Bim4Everyone";

        public static readonly string BinPath = "../../bin";

        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitVersion = "4.8.13.23182";

        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitInstallUrl =
            $@"https://github.com/eirannejad/pyRevit/releases/download/v{pyRevitVersion}+2215/pyRevit_{pyRevitVersion}_signed.exe";

        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitInstallFile =
            Path.Combine(BinPath, $"pyRevit_{pyRevitVersion}_signed.exe");

        public static readonly Version CurrentVersion = new Version(DateTime.Now.ToString("yy.MM.dd"));

        public static readonly string
            AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitPath = Path.Combine(AppDataPath, "pyRevit-Master");
        public static readonly string UninstallerFile = Path.Combine(pyRevitPath, "unins000.exe");
        public static readonly string CoreExtensionPath = Path.Combine(pyRevitPath, "extensions/pyRevitCore.extension");

        public static readonly string AssetsPath = "../../assets";
        public static readonly string BundlesPath = Path.Combine(AppDataPath, "pyRevit");

        public static readonly string ConfigureFileProp = "_configure.bat_";
        public static readonly string ConfigureFile = Path.Combine(BundlesPath, "configure.bat");
        public static readonly string ConfigureAssetFile = Path.Combine(AssetsPath, "configure.bat");
       
        public static readonly string CoreExtensionFileProp = "_extension.json_";
        public static readonly string CoreExtensionFile = Path.Combine(CoreExtensionPath, "extension.json");
        public static readonly string CoreExtensionAssetFile = Path.Combine(AssetsPath, "extension.json");
        
        public static readonly string ExtensionsFileProp = "_extensions.json_";
        public static readonly string ExtensionsFile = Path.Combine(BundlesPath, "extensions.json");
        public static readonly string ExtensionsAssetFile = Path.Combine(BinPath, "extensions.json");
        
        public static readonly string RevitIconAssetFile = Path.Combine(AssetsPath, "revit.png");
        public static readonly string ProductLicenceAssetFile = Path.Combine(AssetsPath, "license.rtf");
        public static readonly string ProductIconAssetFile = Path.Combine(AssetsPath, "Bim4Everyone_transparent.ico");
        public static readonly string WixUIBannerBmp = Path.Combine(AssetsPath, "WixUIBannerBmp.png");
        public static readonly string WixUIDialogBmp = Path.Combine(AssetsPath, "WixUIDialogBmp.png");

        public static readonly string ExtensionsFileUrl =
            @"https://raw.githubusercontent.com/dosymep/BIMExtensions/master/extensions.json";
        
        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitExtensionsPath = Path.Combine(AppDataPath, "pyrevit/Extensions");
        
        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitExtensionsDirPath = $"%AppDataFolder%/pyRevit/Extensions";

        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitVersionProp = "pyRevitVersion";
        
        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitInstalledProp = "pyRevitInstalled";
        
        // ReSharper disable once InconsistentNaming
        public static readonly string pyRevitInstallFileProp = "pyRevitInstallFile";

        public static readonly Condition Install = new Condition(" (NOT Installed) ");
        public static readonly Condition Change = new Condition(" (REMOVE) ");
        public static readonly Condition Repair = new Condition(" (REINSTALL) ");
        public static readonly Condition Remove = new Condition(" (REMOVE=\"ALL\") ");
    }
}