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
            self.Name = $"{Constants.ProductName} {Constants.CurrentTag}";
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
                    new Id(nameof(Actions.UpdateProperties)),
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
                    new Id(nameof(Actions.Uninstall)),
                    Actions.Uninstall,
                    Return.check,
                    When.After,
                    new Step(nameof(Actions.UpdateProperties)),
                    Constants.RemoveCondition
                    | (Constants.InstallCondition & Constants.pyRevitUninstallCondition))
            );

            self.AddXmlElement(
                "Wix/Package/UI",
                "ProgressText",
                $"Action={nameof(Actions.Uninstall)};Message=Удаление pyRevit");

            // Удаляет бандлы
            // Удаляет обязательно до установки
            self.AddAction(
                new ManagedAction(
                    new Id(nameof(Actions.UninstallBundles)),
                    Actions.UninstallBundles,
                    Return.check,
                    When.After,
                    new Step(nameof(Actions.Uninstall)),
                    Constants.InstallCondition | Constants.RemoveCondition)
            );

            self.AddXmlElement(
                "Wix/Package/UI",
                "ProgressText",
                $"Action={nameof(Actions.UninstallBundles)};Message=Удаление расширений платформы");

            // Устанавливает pyRevit
            self.AddAction(new BinaryFileAction(
                new Id(nameof(Constants.pyRevitInstallFileProp)),
                Constants.pyRevitInstallFileProp,
                "/VERYSILENT /CURRENTUSER",
                Return.check,
                When.After,
                new Step(nameof(Actions.UninstallBundles)),
                Constants.InstallCondition & Constants.pyRevitInstallCondition)
            );

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


            // Устанавливает владельца папок на текущего пользователя
            // при установке через SCCM по какой-то причине
            // указывается владельцем всех папок расширений пользователь SYSTEM
            self.AddAction(
                new ManagedAction(
                    Actions.UpdateOwner,
                    Return.check,
                    When.After,
                    Step.InstallFinalize,
                    Constants.ConfigInstallCondition)
            );

            self.AddXmlElement(
                "Wix/Package/UI",
                "ProgressText",
                $"Action={nameof(Actions.UpdateOwner)};Message=Исправление владельца папок расширений");

            return self;
        }

        public static T SetProductProperties<T>(this T self) where T : Project {
            self.AddProperties(
                new Property(Constants.pyRevitVersionProp, "99.99.99"),
                new Property(Constants.pyRevitInstalledProp, "False")
            );

            return self;
        }

        public static T SetProductSocialsProperties<T>(this T self, MsiVariables msiVariables) where T : Project {
            Console.WriteLine("Socials");
            Console.WriteLine($"\tSOCIALS_MAIN: {msiVariables.SocialsMain}");
            Console.WriteLine($"\tSOCIALS_NEWS: {msiVariables.SocialsNews}");
            Console.WriteLine($"\tSOCIALS_DISCUSS: {msiVariables.SocialsDiscuss}");
            Console.WriteLine($"\tSOCIALS_2D: {msiVariables.Socials2D}");
            Console.WriteLine($"\tSOCIALS_BIM: {msiVariables.SocialsBim}");
            Console.WriteLine($"\tSOCIALS_AR: {msiVariables.SocialsAR}");
            Console.WriteLine($"\tSOCIALS_KR: {msiVariables.SocialsKR}");
            Console.WriteLine($"\tSOCIALS_OVVK: {msiVariables.SocialsOVVK}");
            Console.WriteLine($"\tSOCIALS_PAGE: {msiVariables.SocialsPage}");
            Console.WriteLine($"\tSOCIALS_DOWNLOADS: {msiVariables.SocialsDownloads}");

            self.AddProperties(
                new Property("SOCIALS_MAIN", msiVariables.SocialsMain),
                new Property("SOCIALS_NEWS", msiVariables.SocialsNews),
                new Property("SOCIALS_DISCUSS", msiVariables.SocialsDiscuss),
                new Property("SOCIALS_2D", msiVariables.Socials2D),
                new Property("SOCIALS_BIM", msiVariables.SocialsBim),
                new Property("SOCIALS_AR", msiVariables.SocialsAR),
                new Property("SOCIALS_KR", msiVariables.SocialsKR),
                new Property("SOCIALS_OVVK", msiVariables.SocialsOVVK),
                new Property("SOCIALS_PAGE", msiVariables.SocialsPage),
                new Property("SOCIALS_DOWNLOADS", msiVariables.SocialsDownloads)
            );

            return self;
        }

        public static T SetProductSettingsProperties<T>(this T self, MsiVariables msiVariables)
            where T : Project {
            Console.WriteLine("Settings");
            Console.WriteLine($"\tAUTOUPDATE: {msiVariables.AutoUpdate}");
            Console.WriteLine($"\tROCKETMODE: {msiVariables.RocketMode}");
            Console.WriteLine($"\tCHECKUPDATES: {msiVariables.CheckUpdates}");
            Console.WriteLine($"\tUSERCANEXTEND: {msiVariables.UserCanExtend}");
            Console.WriteLine($"\tUSERCANCONFIG: {msiVariables.UserCanConfig}");
            Console.WriteLine($"\tCOREUSERLOCALE: {msiVariables.CoreUserLocale}");
            Console.WriteLine($"\tCORP_NAME: {msiVariables.CorpName}");
            Console.WriteLine($"\tCORP_SETTINGS_PATH: {msiVariables.CorpSettingsPath}");

            self.AddProperties(
                new Property("AUTOUPDATE", msiVariables.AutoUpdate),
                new Property("ROCKETMODE", msiVariables.RocketMode),
                new Property("CHECKUPDATES", msiVariables.CheckUpdates),
                new Property("USERCANEXTEND", msiVariables.UserCanExtend),
                new Property("USERCANCONFIG", msiVariables.UserCanConfig),
                new Property("COREUSERLOCALE", msiVariables.CoreUserLocale),
                new Property("CORP_NAME", msiVariables.CorpName),
                new Property("CORP_SETTINGS_PATH", msiVariables.CorpSettingsPath) {Secure = true}
            );

            return self;
        }

        public static T SetProductTelemetryProperties<T>(this T self, MsiVariables msiVariables) where T : Project {
            Console.WriteLine("Telemetry");
            Console.WriteLine($"\tTELEMETRY_ACTIVE: {msiVariables.TelemetryActive}");
            Console.WriteLine($"\tTELEMETRY_USE_UTC: {msiVariables.TelemetryUseUTC}");
            Console.WriteLine($"\tTELEMETRY_SERVER_URL: {msiVariables.TelemetryServerUrl}");

            self.AddProperties(
                new Property("TELEMETRY_ACTIVE", msiVariables.TelemetryActive),
                new Property("TELEMETRY_USE_UTC", msiVariables.TelemetryUseUTC),
                new Property("TELEMETRY_SERVER_URL", msiVariables.TelemetryServerUrl)
            );

            return self;
        }

        public static T SetProductAppTelemetryProperties<T>(this T self, MsiVariables msiVariables) where T : Project {
            Console.WriteLine("AppTelemetry");
            Console.WriteLine($"\tAPP_TELEMETRY_ACTIVE: {msiVariables.AppTelemetryActive}");
            Console.WriteLine($"\tAPP_TELEMETRY_EVENT_FLAGS: {msiVariables.AppTelemetryEventFlags}");
            Console.WriteLine($"\tAPP_TELEMETRY_SERVER_URL: {msiVariables.AppTelemetryServerUrl}");

            self.AddProperties(
                new Property("APP_TELEMETRY_ACTIVE", msiVariables.AppTelemetryActive),
                new Property("APP_TELEMETRY_EVENT_FLAGS", msiVariables.AppTelemetryEventFlags),
                new Property("APP_TELEMETRY_SERVER_URL", msiVariables.AppTelemetryServerUrl)
            );

            return self;
        }

        public static T SetProductLogTraceProperties<T>(this T self, MsiVariables msiVariables) where T : Project {
            Console.WriteLine("LogTrace");
            Console.WriteLine($"\tLOG_TRACE_ACTIVE: {msiVariables.LogTraceActive}");
            Console.WriteLine($"\tLOG_TRACE_LEVEL: {msiVariables.LogTraceLevel}");
            Console.WriteLine($"\tLOG_TRACE_SERVER_URL: {msiVariables.LogTraceServerUrl}");
            Console.WriteLine($"\tENABLE_OPEN_DOC_TIME: {msiVariables.EnableOpenDocTime}");
            Console.WriteLine($"\tENABLE_SYNC_DOC_TIME: {msiVariables.EnableSyncDocTime}");

            self.AddProperties(
                new Property("LOG_TRACE_ACTIVE", msiVariables.LogTraceActive),
                new Property("LOG_TRACE_LEVEL", msiVariables.LogTraceLevel),
                new Property("LOG_TRACE_SERVER_URL", msiVariables.LogTraceServerUrl),
                new Property("ENABLE_OPEN_DOC_TIME", msiVariables.EnableOpenDocTime),
                new Property("ENABLE_SYNC_DOC_TIME", msiVariables.EnableSyncDocTime)
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
                "extensions paths forget --all", "extensions paths add \"%appdata%\\pyRevit\\Extensions\"",
                "extensions update --all",
            };

            args.ForEach(item =>
                self.CreateConfigureAction(item, "Обновление установленных расширений"));
        }

        private static void CreateConfigureDefaultParams<T>(this T self) where T : Project {
            string[] args = new[] {
                "configs core:user_locale \"[COREUSERLOCALE]\"", "configs rocketmode \"[ROCKETMODE]\"",
                "configs autoupdate \"[AUTOUPDATE]\"", "configs checkupdates \"[CHECKUPDATES]\"",
                "configs usercanextend \"[USERCANEXTEND]\"", "configs usercanconfig \"[USERCANCONFIG]\"",
                "configs corp:name \"[CORP_NAME]\"", "configs corp:settings_path \"[CORP_SETTINGS_PATH]\"",
            };

            args.ForEach(item =>
                self.CreateConfigureAction(item, "Настройка параметров по умолчанию"));
        }

        private static void CreateConfigureSocialsParams<T>(this T self) where T : Project {
            string[] args = new[] {
                "configs socials:tg_main \"[SOCIALS_MAIN]\"", "configs socials:tg_news \"[SOCIALS_NEWS]\"",
                "configs socials:tg_discuss \"[SOCIALS_DISCUSS]\"", "configs socials:tg_2d \"[SOCIALS_2D]\"",
                "configs socials:tg_bim \"[SOCIALS_BIM]\"", "configs socials:tg_ar \"[SOCIALS_AR]\"",
                "configs socials:tg_kr \"[SOCIALS_KR]\"", "configs socials:tg_ovvk \"[SOCIALS_OVVK]\"",
                "configs socials:page_link \"[SOCIALS_PAGE]\"",
                "configs socials:downloads_link \"[SOCIALS_DOWNLOADS]\"",
            };

            args.ForEach(item =>
                self.CreateConfigureAction(item, "Настройка параметров социальных сетей"));
        }

        private static void CreateConfigureTelemetryParams<T>(this T self) where T : Project {
            string[] args = new[] {
                "configs telemetry \"[TELEMETRY_ACTIVE]\"", "configs telemetry utc \"[TELEMETRY_USE_UTC]\"",
                "configs telemetry server \"[TELEMETRY_SERVER_URL]\"",
            };

            args.ForEach(item =>
                self.CreateConfigureAction(item,
                    "Настройка параметров телеметрии", "(TELEMETRY_ACTIVE=\"enable\")"));

            args = new[] {
                "configs apptelemetry \"[APP_TELEMETRY_ACTIVE]\"",
                "configs apptelemetry flags \"[APP_TELEMETRY_EVENT_FLAGS]\"",
                "configs apptelemetry server \"[APP_TELEMETRY_SERVER_URL]\"",
            };

            args.ForEach(item =>
                self.CreateConfigureAction(item,
                    "Настройка параметров телеметрии", "(APP_TELEMETRY_ACTIVE=\"enable\")"));

            args = new[] {
                "configs log_trace:active \"[LOG_TRACE_ACTIVE]\"", "configs log_trace:level \"[LOG_TRACE_LEVEL]\"",
                "configs log_trace:server_name \"[LOG_TRACE_SERVER_URL]\"",
                "configs log_trace:enable_open_doc_time \"[ENABLE_OPEN_DOC_TIME]\"",
                "configs log_trace:enable_sync_doc_time \"[ENABLE_SYNC_DOC_TIME]\"",
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
                "configs pyRevitCore.extension:disabled true", "configs pyRevitCore.extension:private_repo true",
                "configs pyRevitCore.extension:username \"\"", "configs pyRevitCore.extension:password \"\"",
                "configs pyRevitDevHooks.extension:disabled true",
                "configs pyRevitDevHooks.extension:private_repo true",
                "configs pyRevitDevHooks.extension:username \"\"", "configs pyRevitDevHooks.extension:password \"\"",
                "configs pyRevitDevTools.extension:disabled true",
                "configs pyRevitDevTools.extension:private_repo true",
                "configs pyRevitDevTools.extension:username \"\"", "configs pyRevitDevTools.extension:password \"\"",
                "configs pyRevitTags.extension:disabled true", "configs pyRevitTags.extension:private_repo true",
                "configs pyRevitTags.extension:username \"\"", "configs pyRevitTags.extension:password \"\"",
                "configs pyRevitTemplates.extension:disabled true",
                "configs pyRevitTemplates.extension:private_repo true",
                "configs pyRevitTemplates.extension:username \"\"", "configs pyRevitTemplates.extension:password \"\"",
                "configs pyRevitTools.extension:disabled true", "configs pyRevitTools.extension:private_repo true",
                "configs pyRevitTools.extension:username \"\"", "configs pyRevitTools.extension:password \"\"",
                "configs pyRevitTutor.extension:disabled true", "configs pyRevitTutor.extension:private_repo true",
                "configs pyRevitTutor.extension:username \"\"", "configs pyRevitTutor.extension:password \"\"",
            };

            args.ForEach(item =>
                self.CreateConfigureAction(item, "Отключение встроенных расширений pyRevit"));
        }

        private static void CreateAttachRevits<T>(this T self) where T : Project {
            string[] args = new[] {
                "detach --all", "attach master default 2022", "attach master default 2023",
                "attach master default 2024",
            };

            args.ForEach(item =>
                self.CreateConfigureAction(item, "Прикрепление поддерживаемых версий Revit"));
        }

        private static void CreateConfigureAction<T>(this T self,
            string args, string message, Condition condition = null) where T : Project {
            if(condition == null) {
                condition = Constants.ConfigInstallCondition;
            } else {
                condition = condition & Constants.ConfigInstallCondition;
            }

            Step step = Step.InstallFinalize;
            if(_configActionNameId > 0) {
                step = new Step(GeneratePreviousActionId("_ConfigureAction_"));
            }

            WixQuietExecAction action = new WixQuietExecAction(
                Constants.pyRevitCliPath,
                args,
                Return.check,
                When.After,
                step,
                condition) {Id = GenerateActionId("_ConfigureAction_")};

            self.AddAction(action);
            self.AddXmlElement(
                "Wix/Package/UI",
                "ProgressText",
                $"Action={action.Id};Message={message}");
        }

        private static string GenerateActionId(string name) {
            return $"{name}.{_configActionNameId++}";
        }

        private static string GeneratePreviousActionId(string name) {
            return $"{name}.{_configActionNameId - 1}";
        }
    }
}