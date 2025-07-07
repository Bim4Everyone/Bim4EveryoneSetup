using System;

namespace BIM4EveryoneSetup {
    internal static class BuildVariables {
        public static string BotAccessToken => Environment.GetEnvironmentVariable("BOT_ACCESS_TOKEN");
        
        public static string Autoupdate => Environment.GetEnvironmentVariable("AUTOUPDATE") ?? "enable";
        public static string Rocketmode => Environment.GetEnvironmentVariable("ROCKETMODE") ?? "enable";
        public static string Checkupdates => Environment.GetEnvironmentVariable("CHECKUPDATES") ?? "enable";
        public static string UserCanExtend => Environment.GetEnvironmentVariable("USERCANEXTEND") ?? "yes";
        public static string UserCanConfig => Environment.GetEnvironmentVariable("USERCANCONFIG") ?? "yes";
        public static string CoreUserLocale => Environment.GetEnvironmentVariable("COREUSERLOCALE") ?? "ru";
        
        public static string CorpName => Environment.GetEnvironmentVariable("CORP_NAME") ?? "Bim4Everyone";
        public static string CorpSettingsPath => Environment.GetEnvironmentVariable("CORP_SETTINGS_PATH");
        
        public static string TelemetryActive => Environment.GetEnvironmentVariable("TELEMETRY_ACTIVE") ?? "disable";
        public static string TelemetryUseUTC => Environment.GetEnvironmentVariable("TELEMETRY_USE_UTC") ?? "yes";
        public static string TelemetryServerUrl => Environment.GetEnvironmentVariable("TELEMETRY_SERVER_URL") ?? "localhost";
        
        public static string AppTelemetryActive => Environment.GetEnvironmentVariable("APP_TELEMETRY_ACTIVE") ?? "disable";
        public static string AppTelemetryEventFlags => Environment.GetEnvironmentVariable("APP_TELEMETRY_EVENT_FLAGS") ?? "0x4000400004003";
        public static string AppTelemetryServerUrl => Environment.GetEnvironmentVariable("APP_TELEMETRY_SERVER_URL") ?? "localhost";
        
        public static string LogTraceActive => Environment.GetEnvironmentVariable("LOG_TRACE_ACTIVE") ?? "disable";
        public static string LogTraceLevel => Environment.GetEnvironmentVariable("LOG_TRACE_LEVEL") ?? "Information";
        public static string LogTraceServerUrl => Environment.GetEnvironmentVariable("LOG_TRACE_SERVER_URL") ?? "localhost";
        public static string EnableOpenDocTime => Environment.GetEnvironmentVariable("ENABLE_OPEN_DOC_TIME") ?? "true";
        public static string EnableSyncDocTime => Environment.GetEnvironmentVariable("ENABLE_SYNC_DOC_TIME") ?? "true";
        
        public static string SocialsMain => Environment.GetEnvironmentVariable("SOCIALS_MAIN") ?? "https://t.me/bim4everyone_group";
        public static string SocialsNews => Environment.GetEnvironmentVariable("SOCIALS_NEWS") ?? "https://t.me/bim4everyone_news";
        public static string SocialsDiscuss => Environment.GetEnvironmentVariable("SOCIALS_DISCUSS") ?? "https://t.me/bim4everyone_discuss";
        public static string Socials2D => Environment.GetEnvironmentVariable("SOCIALS_2D") ?? "https://t.me/bim4everyone_group/12";
        public static string SocialsBim => Environment.GetEnvironmentVariable("SOCIALS_BIM") ?? "https://t.me/bim4everyone_group/11";
        public static string SocialsAR => Environment.GetEnvironmentVariable("SOCIALS_AR") ?? "https://t.me/bim4everyone_group/8";
        public static string SocialsKR => Environment.GetEnvironmentVariable("SOCIALS_KR") ?? "https://t.me/bim4everyone_group/7";
        public static string SocialsOVVK => Environment.GetEnvironmentVariable("SOCIALS_OVVK") ?? "https://t.me/bim4everyone_group/6";
        public static string SocialsPage => Environment.GetEnvironmentVariable("SOCIALS_PAGE") ?? "https://bim4everyone.com/";
        public static string SocialsDownloads => Environment.GetEnvironmentVariable("SOCIALS_DOWNLOADS") ?? "https://github.com/Bim4Everyone/Bim4EveryoneSetup/releases/latest";
    }
}