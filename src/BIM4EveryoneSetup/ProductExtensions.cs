using System;
using System.Linq;
using System.Text;

using WixSharp;
using WixSharp.CommonTasks;

namespace BIM4EveryoneSetup {
    internal static class ProductExtensions {
        private static int _configActionNameId = 0;
        
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
                "configs pyRevitBundlesCreatorExtension.extension:disabled true",
                "configs pyRevitBundlesCreatorExtension.extension:private_repo true",
                "configs pyRevitBundlesCreatorExtension.extension:username \"\"",
                "configs pyRevitBundlesCreatorExtension.extension:password \"\"",
                "configs pyRevitCore.extension:disabled true", 
                "configs pyRevitCore.extension:private_repo true",
                "configs pyRevitCore.extension:username \"\"",
                "configs pyRevitCore.extension:password \"\"",
                "configs pyRevitDevHooks.extension:disabled true",
                "configs pyRevitDevHooks.extension:private_repo true",
                "configs pyRevitDevHooks.extension:username \"\"", 
                "configs pyRevitDevHooks.extension:password \"\"",
                "configs pyRevitDevTools.extension:disabled true",
                "configs pyRevitDevTools.extension:private_repo true",
                "configs pyRevitDevTools.extension:username \"\"",
                "configs pyRevitDevTools.extension:password \"\"",
                "configs pyRevitTags.extension:disabled true", 
                "configs pyRevitTags.extension:private_repo true",
                "configs pyRevitTags.extension:username \"\"", 
                "configs pyRevitTags.extension:password \"\"",
                "configs pyRevitTemplates.extension:disabled true",
                "configs pyRevitTemplates.extension:private_repo true",
                "configs pyRevitTemplates.extension:username \"\"", 
                "configs pyRevitTemplates.extension:password \"\"",
                "configs pyRevitTools.extension:disabled true", 
                "configs pyRevitTools.extension:private_repo true",
                "configs pyRevitTools.extension:username \"\"",
                "configs pyRevitTools.extension:password \"\"",
                "configs pyRevitTutor.extension:disabled true",
                "configs pyRevitTutor.extension:private_repo true",
                "configs pyRevitTutor.extension:username \"\"",
                "configs pyRevitTutor.extension:password \"\"",
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

            WixQuietExecAction action = new WixQuietExecAction(
                Constants.pyRevitCliPath, args,
                Return.check, When.After, Step.InstallFinalize, condition) {Id = GenerateActionId("_ConfigureAction_")};

            self.AddAction(action);
            self.AddXmlElement(
                "Wix/Package/UI",
                "ProgressText",
                $"Action={action.Id};Message={message}");
        }

        private static string GenerateActionId(string name) {
            return $"{name}.{_configActionNameId++}";
        }
    }
}