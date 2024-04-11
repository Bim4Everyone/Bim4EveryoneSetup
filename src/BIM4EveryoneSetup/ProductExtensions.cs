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

        public static T SetProductProperties<T>(this T self) where T : Project {
            self.AddProperties(
                new Property(Constants.pyRevitVersionProp, "0.0.0"),
                new Property(Constants.pyRevitInstalledProp, "False")
            );
            
            return self;
        }
        
        public static T SetProductSocialsProperties<T>(this T self) where T : Project {
            Console.WriteLine("Socials");
            Console.WriteLine($"\tSOCIALS_MAIN: {Environment.GetEnvironmentVariable("SOCIALS_MAIN")}");
            Console.WriteLine($"\tSOCIALS_NEWS: {Environment.GetEnvironmentVariable("SOCIALS_NEWS")}");
            Console.WriteLine($"\tSOCIALS_DISCUSS: {Environment.GetEnvironmentVariable("SOCIALS_DISCUSS")}");
            Console.WriteLine($"\tSOCIALS_2D: {Environment.GetEnvironmentVariable("SOCIALS_2D")}");
            Console.WriteLine($"\tSOCIALS_BIM: {Environment.GetEnvironmentVariable("SOCIALS_BIM")}");
            Console.WriteLine($"\tSOCIALS_AR: {Environment.GetEnvironmentVariable("SOCIALS_AR")}");
            Console.WriteLine($"\tSOCIALS_KR: {Environment.GetEnvironmentVariable("SOCIALS_KR")}");
            Console.WriteLine($"\tSOCIALS_OVVK: {Environment.GetEnvironmentVariable("SOCIALS_OVVK")}");
            Console.WriteLine($"\tSOCIALS_PAGE: {Environment.GetEnvironmentVariable("SOCIALS_PAGE")}");
            Console.WriteLine($"\tSOCIALS_DOWNLOADS: {Environment.GetEnvironmentVariable("SOCIALS_DOWNLOADS")}");

            self.AddProperties(
                new Property("SOCIALS_MAIN", Environment.GetEnvironmentVariable("SOCIALS_MAIN")),
                new Property("SOCIALS_NEWS", Environment.GetEnvironmentVariable("SOCIALS_NEWS")),
                new Property("SOCIALS_DISCUSS", Environment.GetEnvironmentVariable("SOCIALS_DISCUSS")),
                new Property("SOCIALS_2D", Environment.GetEnvironmentVariable("SOCIALS_2D")),
                new Property("SOCIALS_BIM", Environment.GetEnvironmentVariable("SOCIALS_BIM")),
                new Property("SOCIALS_AR", Environment.GetEnvironmentVariable("SOCIALS_AR")),
                new Property("SOCIALS_KR", Environment.GetEnvironmentVariable("SOCIALS_KR")),
                new Property("SOCIALS_OVVK", Environment.GetEnvironmentVariable("SOCIALS_OVVK")),
                new Property("SOCIALS_PAGE", Environment.GetEnvironmentVariable("SOCIALS_PAGE")),
                new Property("SOCIALS_DOWNLOADS", Environment.GetEnvironmentVariable("SOCIALS_DOWNLOADS"))
            );
            
            return self;
        }
        
        public static T SetProductSettingsProperties<T>(this T self) where T : Project {
            Console.WriteLine("Settings");
            Console.WriteLine($"\tAUTOUPDATE: {Environment.GetEnvironmentVariable("AUTOUPDATE")}");
            Console.WriteLine($"\tROCKETMODE: {Environment.GetEnvironmentVariable("ROCKETMODE")}");
            Console.WriteLine($"\tCHECKUPDATES: {Environment.GetEnvironmentVariable("CHECKUPDATES")}");
            Console.WriteLine($"\tUSERCANEXTEND: {Environment.GetEnvironmentVariable("USERCANEXTEND")}");
            Console.WriteLine($"\tUSERCANCONFIG: {Environment.GetEnvironmentVariable("USERCANCONFIG")}");
            Console.WriteLine($"\tCOREUSERLOCALE: {Environment.GetEnvironmentVariable("COREUSERLOCALE")}");
            Console.WriteLine($"\tCORP_NAME: {Environment.GetEnvironmentVariable("CORP_NAME")}");
            Console.WriteLine($"\tCORP_SETTINGS_PATH: {Environment.GetEnvironmentVariable("CORP_SETTINGS_PATH")}");
            
            self.AddProperties(
                new Property("AUTOUPDATE", Environment.GetEnvironmentVariable("AUTOUPDATE")),
                new Property("ROCKETMODE", Environment.GetEnvironmentVariable("ROCKETMODE")),
                new Property("CHECKUPDATES", Environment.GetEnvironmentVariable("CHECKUPDATES")),
                new Property("USERCANEXTEND", Environment.GetEnvironmentVariable("USERCANEXTEND")),
                new Property("USERCANCONFIG", Environment.GetEnvironmentVariable("USERCANCONFIG")),
                new Property("COREUSERLOCALE", Environment.GetEnvironmentVariable("COREUSERLOCALE")),
                new Property("CORP_NAME", Environment.GetEnvironmentVariable("CORP_NAME")),
                new Property("CORP_SETTINGS_PATH", Environment.GetEnvironmentVariable("CORP_SETTINGS_PATH"))
            );
            
            return self;
        }
        
        public static T SetProductTelemetryProperties<T>(this T self) where T : Project {
            Console.WriteLine("Telemetry");
            Console.WriteLine($"\tTELEMETRY_ACTIVE: {Environment.GetEnvironmentVariable("TELEMETRY_ACTIVE")}");
            Console.WriteLine($"\tTELEMETRY_USE_UTC: {Environment.GetEnvironmentVariable("TELEMETRY_USE_UTC")}");
            Console.WriteLine($"\tTELEMETRY_SERVER_URL: {Environment.GetEnvironmentVariable("TELEMETRY_SERVER_URL")}");
            
            self.AddProperties(
                new Property("TELEMETRY_ACTIVE", Environment.GetEnvironmentVariable("TELEMETRY_ACTIVE")),
                new Property("TELEMETRY_USE_UTC", Environment.GetEnvironmentVariable("TELEMETRY_USE_UTC")),
                new Property("TELEMETRY_SERVER_URL", Environment.GetEnvironmentVariable("TELEMETRY_SERVER_URL"))
            );
            
            return self;
        }
        
        public static T SetProductAppTelemetryProperties<T>(this T self) where T : Project {
            Console.WriteLine("AppTelemetry");
            Console.WriteLine($"\tAPP_TELEMETRY_ACTIVE: {Environment.GetEnvironmentVariable("APP_TELEMETRY_ACTIVE")}");
            Console.WriteLine($"\tAPP_TELEMETRY_EVENT_FLAGS: {Environment.GetEnvironmentVariable("APP_TELEMETRY_EVENT_FLAGS")}");
            Console.WriteLine($"\tAPP_TELEMETRY_SERVER_URL: {Environment.GetEnvironmentVariable("APP_TELEMETRY_SERVER_URL")}");
            
            self.AddProperties(
                new Property("APP_TELEMETRY_ACTIVE", Environment.GetEnvironmentVariable("APP_TELEMETRY_ACTIVE")),
                new Property("APP_TELEMETRY_EVENT_FLAGS", Environment.GetEnvironmentVariable("APP_TELEMETRY_EVENT_FLAGS")),
                new Property("APP_TELEMETRY_SERVER_URL", Environment.GetEnvironmentVariable("APP_TELEMETRY_SERVER_URL"))
            );
            
            return self;
        }
        
        public static T SetProductLogTraceProperties<T>(this T self) where T : Project {
            Console.WriteLine("LogTrace");
            Console.WriteLine($"\tLOG_TRACE_ACTIVE: {Environment.GetEnvironmentVariable("LOG_TRACE_ACTIVE")}");
            Console.WriteLine($"\tLOG_TRACE_LEVEL: {Environment.GetEnvironmentVariable("LOG_TRACE_LEVEL")}");
            Console.WriteLine($"\tLOG_TRACE_SERVER_URL: {Environment.GetEnvironmentVariable("LOG_TRACE_SERVER_URL")}");
            
            self.AddProperties(
                new Property("LOG_TRACE_ACTIVE", Environment.GetEnvironmentVariable("LOG_TRACE_ACTIVE")),
                new Property("LOG_TRACE_LEVEL", Environment.GetEnvironmentVariable("LOG_TRACE_LEVEL")),
                new Property("LOG_TRACE_SERVER_URL", Environment.GetEnvironmentVariable("LOG_TRACE_SERVER_URL"))
            );
            
            return self;
        }

        public static T SetProductConfiguration<T>(this T self) where T : Project {
            // Прикрепление поддерживаемых версий Revit
            self.CreateAttachRevits();

            // Обновление установленных расширений
            self.CreateConfigureUpdateExtensions();

            // Настройка дефолтных параметров
            self.CreateConfigureDefaultParams();
            
            // Настройка социальных сетей
            self.CreateConfigureSocialsParams();
            
            // Настройка телеметрии
            self.CreateConfigureTelemetryParams();

            // Отключение встроенных расширений pyRevit
            self.CreateConfigureDisableBuiltinExtensions();

            return self;
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
                "configs core:user_locale [COREUSERLOCALE]",
                "configs rocketmode [ROCKETMODE]",
                "configs autoupdate [AUTOUPDATE]",
                "configs checkupdates [CHECKUPDATES]", 
                "configs usercanextend [USERCANEXTEND]", 
                "configs usercanconfig [USERCANCONFIG]",
                "configs corp:name [CORP_NAME]",
                "configs corp:settings_path [CORP_SETTINGS_PATH]",
            };

            args.ForEach(item =>
                self.CreateConfigureAction(item, "Настройка параметров по умолчанию"));
        }
        
        private static void CreateConfigureSocialsParams<T>(this T self) where T : Project {
            string[] args = new[] {
                "configs socials:tg_main [SOCIALS_MAIN]",
                "configs socials:tg_news [SOCIALS_NEWS]",
                "configs socials:tg_discuss [SOCIALS_DISCUSS]",
                "configs socials:tg_2d [SOCIALS_2D]",
                "configs socials:tg_bim [SOCIALS_BIM]",
                "configs socials:tg_ar [SOCIALS_AR]",
                "configs socials:tg_kr [SOCIALS_KR]",
                "configs socials:tg_ovvk [SOCIALS_OVVK]",
                "configs socials:page_link [SOCIALS_PAGE]",
                "configs socials:downloads_link [SOCIALS_DOWNLOADS]",
            };

            args.ForEach(item =>
                self.CreateConfigureAction(item, "Настройка параметров социальных сетей"));
        }
        
        private static void CreateConfigureTelemetryParams<T>(this T self) where T : Project {
            string[] args = new[] {
                "configs telemetry [TELEMETRY_ACTIVE]", 
                "configs telemetry utc [TELEMETRY_USE_UTC]",
                "configs telemetry server [TELEMETRY_SERVER_URL]",
            };
            
            args.ForEach(item =>
                self.CreateConfigureAction(item, 
                    "Настройка параметров телеметрии", "(TELEMETRY_ACTIVE=\"enable\")"));
            
            args = new[] {
                "configs apptelemetry [APP_TELEMETRY_ACTIVE]", 
                "configs apptelemetry flags [APP_TELEMETRY_EVENT_FLAGS]",
                "configs apptelemetry server [APP_TELEMETRY_SERVER_URL]",
            };
            
            args.ForEach(item =>
                self.CreateConfigureAction(item, 
                    "Настройка параметров телеметрии", "(APP_TELEMETRY_ACTIVE=\"enable\")"));
            
            args = new[] {
                "configs log_trace:active [LOG_TRACE_ACTIVE]",
                "configs log_trace:level [LOG_TRACE_LEVEL]",
                "configs log_trace:server_name [LOG_TRACE_SERVER_URL]",
            };

            args.ForEach(item =>
                self.CreateConfigureAction(item, 
                    "Настройка параметров телеметрии", "(LOG_TRACE_ACTIVE=\"enable\")"));
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
                "attach master default 2020", 
                "attach master default 2021",
                "attach master default 2022",
                "attach master default 2023",
                "attach master default 2024",
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