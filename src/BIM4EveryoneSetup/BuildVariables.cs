using System;

namespace BIM4EveryoneSetup {
    internal static class BuildVariables {
        public static string BotAccessToken => Environment.GetEnvironmentVariable("B4E_BOT_ACCESS_TOKEN");
        
        public static string Autoupdate => Environment.GetEnvironmentVariable("B4E_AUTOUPDATE") ?? "enable";
        public static string Rocketmode => Environment.GetEnvironmentVariable("B4E_OCKETMODE") ?? "enable";
        public static string Checkupdates => Environment.GetEnvironmentVariable("B4E_CHECKUPDATES") ?? "enable";
        public static string UserCanExtend => Environment.GetEnvironmentVariable("B4E_USERCANEXTEND") ?? "yes";
        public static string UserCanConfig => Environment.GetEnvironmentVariable("B4E_USERCANCONFIG") ?? "yes";
        public static string CoreUserLocale => Environment.GetEnvironmentVariable("B4E_COREUSERLOCALE") ?? "ru";
        
        public static string CorpName => Environment.GetEnvironmentVariable("B4E_CORP_NAME") ?? "Bim4Everyone";
        public static string CorpSettingsPath => Environment.GetEnvironmentVariable("B4E_CORP_SETTINGS_PATH");
        
        public static string TelemetryActive => Environment.GetEnvironmentVariable("B4E_TELEMETRY_ACTIVE") ?? "disable";
        public static string TelemetryUseUTC => Environment.GetEnvironmentVariable("B4E_TELEMETRY_USE_UTC") ?? "yes";
        public static string TelemetryServerUrl => Environment.GetEnvironmentVariable("B4E_TELEMETRY_SERVER_URL") ?? "localhost";
        
        public static string AppTelemetryActive => Environment.GetEnvironmentVariable("B4E_APP_TELEMETRY_ACTIVE") ?? "disable";
        public static string AppTelemetryEventFlags => Environment.GetEnvironmentVariable("B4E_APP_TELEMETRY_EVENT_FLAGS") ?? "0x4000400004003";
        public static string AppTelemetryServerUrl => Environment.GetEnvironmentVariable("B4E_APP_TELEMETRY_SERVER_URL") ?? "localhost";
        
        public static string LogTraceActive => Environment.GetEnvironmentVariable("B4E_LOG_TRACE_ACTIVE") ?? "disable";
        public static string LogTraceLevel => Environment.GetEnvironmentVariable("B4E_LOG_TRACE_LEVEL") ?? "Information";
        public static string LogTraceServerUrl => Environment.GetEnvironmentVariable("B4E_LOG_TRACE_SERVER_URL") ?? "localhost";
        public static string EnableOpenDocTime => Environment.GetEnvironmentVariable("B4E_ENABLE_OPEN_DOC_TIME") ?? "true";
        public static string EnableSyncDocTime => Environment.GetEnvironmentVariable("B4E_ENABLE_SYNC_DOC_TIME") ?? "true";
        
        public static string SocialsMain => Environment.GetEnvironmentVariable("B4E_SOCIALS_MAIN") ?? "https://t.me/bim4everyone_group";
        public static string SocialsNews => Environment.GetEnvironmentVariable("B4E_SOCIALS_NEWS") ?? "https://t.me/bim4everyone_news";
        public static string SocialsDiscuss => Environment.GetEnvironmentVariable("B4E_SOCIALS_DISCUSS") ?? "https://t.me/bim4everyone_discuss";
        public static string Socials2D => Environment.GetEnvironmentVariable("B4E_SOCIALS_2D") ?? "https://t.me/bim4everyone_group/12";
        public static string SocialsBim => Environment.GetEnvironmentVariable("B4E_SOCIALS_BIM") ?? "https://t.me/bim4everyone_group/11";
        public static string SocialsAR => Environment.GetEnvironmentVariable("B4E_SOCIALS_AR") ?? "https://t.me/bim4everyone_group/8";
        public static string SocialsKR => Environment.GetEnvironmentVariable("B4E_SOCIALS_KR") ?? "https://t.me/bim4everyone_group/7";
        public static string SocialsOVVK => Environment.GetEnvironmentVariable("B4E_SOCIALS_OVVK") ?? "https://t.me/bim4everyone_group/6";
        public static string SocialsPage => Environment.GetEnvironmentVariable("B4E_SOCIALS_PAGE") ?? "https://bim4everyone.com/";
        public static string SocialsDownloads => Environment.GetEnvironmentVariable("B4E_SOCIALS_DOWNLOADS") ?? "https://github.com/Bim4Everyone/Bim4EveryoneSetup/releases/latest";
    }
}