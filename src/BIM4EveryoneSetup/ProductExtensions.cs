using System.Text;

using WixSharp;
using WixSharp.CommonTasks;

namespace BIM4EveryoneSetup {
    internal static class ProductExtensions {
        public static T SetProductInfo<T>(this T self) where T : Project {
            self.GUID = Constants.ProductId;
            self.UpgradeCode = Constants.ProductId;
            self.Name = Constants.ProductName;
            self.Description = Constants.ProductDescription;
            self.Version = Constants.CurrentVersion;
            self.LicenceFile = Constants.ProductLicenceAssetFile;
            
            self.ControlPanelInfo = new ProductInfo() {
                Name = Constants.ProductName,
                Manufacturer = Constants.ProductName,
                Readme = Constants.ProductHelpUrl,
                Contact = Constants.ProductContacts,
                Comments = Constants.ProductDescription,
                HelpLink = Constants.ProductHelpUrl,
                UrlInfoAbout = Constants.ProductAboutUrl,
                ProductIcon = Constants.ProductIconAssetFile
            };

            return self;
        }

        public static T SetProductSettings<T>(this T self) where T : Project {
            self.Codepage = "1251";
            self.Language = "ru-RU";
            self.Encoding = Encoding.UTF8;
            self.Platform = Platform.x64;
            self.Scope = InstallScope.perUser;
            return self;
        }

        public static T SetProductUI<T>(this T self) where T : Project {
            self.UI = WUI.WixUI_FeatureTree;
            return self;
        }

        public static T SetBinaries<T>(this T self) where T : Project {
            // Установщик pyRevit (нужно ссылку на скачивание обновлять)
            self.AddBinaries(new Binary(
                new Id(Constants.pyRevitInstallFileProp), Constants.pyRevitInstallFile));

            // Батник дефолтной настройки pyRevit
            self.AddBinaries(new Binary(
                new Id(Constants.ConfigureFileProp), Constants.ConfigureAssetFile));

            // Файл InvokableDLLEngine.cs, нужно будет удалить, когда pyRevit обновим на 4.8.14
            self.AddBinaries(new Binary(
                new Id(Constants.InvokeDllFileProp), Constants.InvokeDllAssetFile));
            
            // Файл extension.json, нужно будет удалить, когда pyRevit обновим на 4.8.14
            self.AddBinaries(new Binary(
                new Id(Constants.CoreExtensionFileProp), Constants.CoreExtensionAssetFile));

            // Файл настроек расширений платформы
            self.AddBinaries(new Binary(
                new Id(Constants.ExtensionsFileProp), Constants.ExtensionsAssetFile));
            return self;
        }

        public static T SetProductActions<T>(this T self) where T : Project {
            // Обновление свойств
            // (определяет стандартные свойства)
            self.AddAction(
                new ManagedAction(
                    Actions.UpdateProperties,
                    Return.check,
                    When.Before,
                    Step.InstallExecute,
                    Condition.Always)
            );

            self.AddXmlElement(
                "Wix/Package/UI", 
                "ProgressText", 
                $"Action={nameof(Actions.UpdateProperties)};Message=Обновление свойств");
            
            // Удаляет pyRevit
            // Удаляет обязательно до установки
            self.AddAction(
                new ManagedAction(
                    Actions.Uninstall,
                    Return.check,
                    When.Before,
                    Step.InstallExecute,
                    Constants.Remove
                    + "OR"
                    + "(" + Constants.Install
                    + $"AND {Constants.pyRevitVersionProp} < \"{Constants.pyRevitVersion}\")")
            );
            
            self.AddXmlElement(
                "Wix/Package/UI", 
                "ProgressText", 
                $"Action={nameof(Actions.Uninstall)};Message=Удаление pyRevit");
            
            // Удаляет бандлы
            // Удаляет обязательно до установки
            self.AddAction(
                new ManagedAction(
                    Actions.UninstallBundles,
                    Return.check,
                    When.Before,
                    Step.InstallExecute,
                    Constants.Install
                    + " OR "
                    + Constants.Remove)
            );
            
            self.AddXmlElement(
                "Wix/Package/UI", 
                "ProgressText", 
                $"Action={nameof(Actions.UninstallBundles)};Message=Удаление расширений платформы");
            
            // Устанавливает pyRevit
            self.AddAction(new BinaryFileAction(
                Constants.pyRevitInstallFileProp,
                "/VERYSILENT /CURRENTUSER",
                Return.check,
                When.Before,
                Step.InstallExecute,
                Constants.Install +
                $"AND {Constants.pyRevitVersionProp} < \"{Constants.pyRevitVersion}\""));

            self.AddXmlElement(
                "Wix/Package/UI", 
                "ProgressText", 
                $"Action={Constants.pyRevitInstallFileProp};Message=Установка pyRevit");

            // Удаляет полностью папки
            // расширений при модификации
            self.AddAction(
                new ManagedAction(
                    Actions.ModifyExtensions,
                    Return.check,
                    When.Before,
                    Step.InstallFinalize,
                    Constants.Change)
            );
            
            self.AddXmlElement(
                "Wix/Package/UI", 
                "ProgressText", 
                $"Action={nameof(Actions.ModifyExtensions)};Message=Модификация расширений платформы");

            // Перед восстановлением
            // полностью удаляет папки расширений
            self.AddAction(
                new ManagedAction(
                    Actions.RepairExtensions,
                    Return.check,
                    When.Before,
                    Step.InstallExecute,
                    Constants.Repair)
            );
            
            self.AddXmlElement(
                "Wix/Package/UI", 
                "ProgressText", 
                $"Action={nameof(Actions.RepairExtensions)};Message=Восстановление расширений платформы");

            // Конфигурирует pyRevit
            self.AddAction(
                new ManagedAction(
                    Actions.Configure,
                    Return.check,
                    When.After,
                    Step.InstallFinalize,
                    Constants.Repair
                    + "OR"
                    + Constants.Install)
            );
            
            self.AddXmlElement(
                "Wix/Package/UI", 
                "ProgressText", 
                $"Action={nameof(Actions.Configure)};Message=Конфигурация pyRevit");

            return self;
        }
    }
}