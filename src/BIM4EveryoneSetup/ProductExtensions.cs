using System.Linq;
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
                    Constants.RemoveCondition
                    + "OR"
                    + "(" + Constants.InstallCondition
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
                    Constants.InstallCondition
                    + " OR "
                    + Constants.RemoveCondition)
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
                Constants.InstallCondition +
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
                    Constants.ChangeCondition)
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
                    Constants.RepairCondition)
            );

            self.AddXmlElement(
                "Wix/Package/UI",
                "ProgressText",
                $"Action={nameof(Actions.RepairExtensions)};Message=Восстановление расширений платформы");

            return self;
        }

        public static void SetProductConfiguration<T>(this T self) where T : Project {
            // Прикрепление поддерживаемых версий Revit
            self.CreateAttachRevits();

            // Обновление установленных расширений
            self.CreateConfigureUpdateExtensions();

            // Настройка дефолтных параметров
            self.CreateConfigureDefaultParams();

            // Отключение встроенных расширений pyRevit
            self.CreateConfigureDisableBuiltinExtensions();
        }

        private static void CreateConfigureUpdateExtensions<T>(this T self) where T : Project {
            string[] args = new[] {
                "extensions paths forget --all",
                "extensions paths add \"%appdata%\\pyRevit\\Extensions\"",
                "extensions update --all",
            };

            args.ForEach(item => 
                self.CreateConfigureAction(item, "Обновление установленных расширений"));
        }
        
        private static void CreateConfigureDefaultParams<T>(this T self) where T : Project {
            string[] args = new[] {
                "configs core:user_locale ru", 
                "configs rocketmode enable", 
                "configs autoupdate enable",
                "configs checkupdates enable",
                "configs usercanextend yes",
                "configs usercanconfig yes",
            };

            args.ForEach(item => 
                self.CreateConfigureAction(item, "Настройка дефолтных параметров"));
        }
        
        private static void CreateConfigureDisableBuiltinExtensions<T>(this T self) where T : Project {
            string[] args = new[] {
                "extensions disable pyRevitBundlesCreatorExtension.extension",
                "extensions disable pyRevitCore.extension",
                "extensions disable pyRevitDevHooks.extension",
                "extensions disable pyRevitDevTools.extension",
                "extensions disable pyRevitTags.extension",
                "extensions disable pyRevitTemplates.extension",
                "extensions disable pyRevitTools.extension",
                "extensions disable pyRevitTutor.extension",
            };

            args.ForEach(item =>
                self.CreateConfigureAction(item, "Отключение встроенных расширений pyRevit"));
        }
        
        private static void CreateAttachRevits<T>(this T self) where T : Project {
            string[] args = new[] {
                "detach --all",
                "attach master 277 2020",
                "attach master 277 2021",
                "attach master 277 2022",
                "attach master 277 2023",
                "attach master 277 2024",
            };

             args.ForEach(item => 
                 self.CreateConfigureAction(item, "Прикрепление поддерживаемых версий Revit"));
        }

        private static void CreateConfigureAction<T>(this T self,
            string args, string message, Condition condition = null) where T : Project {
            condition = condition ?? Constants.ConfigInstallCondition;

            Action action = new WixQuietExecAction(
                Constants.pyRevitCliPath, args,
                Return.check, When.After, Step.InstallFinalize, condition);

            self.AddAction(action);
            self.AddXmlElement(
                "Wix/Package/UI",
                "ProgressText",
                $"Action={action.Id};Message={message}");
        }
    }
}