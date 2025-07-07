using System;

namespace BIM4EveryoneSetup {
    internal sealed class MsiVariables {
        public static readonly string BotAccessToken =
            Environment.GetEnvironmentVariable("B4E_BOT_ACCESS_TOKEN");
        
        public static readonly string MsiVariablesFile =
            Environment.GetEnvironmentVariable("B4E_MSI_VARIABLES_PATH") ?? Constants.MsiVariablesFile;

        public string AutoUpdate { get; set; } = "enable";
        public string RocketMode { get; set; } = "enable";
        public string CheckUpdates { get; set; } = "enable";
        public string UserCanExtend { get; set; } = "yes";
        public string UserCanConfig { get; set; } = "yes";
        public string CoreUserLocale { get; set; } = "ru";

        public string CorpName { get; set; } = "Bim4Everyone";
        public string CorpSettingsPath { get; set; } = "\"\"";

        public string TelemetryActive { get; set; } = "disable";
        public string TelemetryUseUTC { get; set; } = "yes";
        public string TelemetryServerUrl { get; set; } = "localhost";

        public string AppTelemetryActive { get; set; } = "disable";
        public string AppTelemetryEventFlags { get; set; } = "0x4000400004003";
        public string AppTelemetryServerUrl { get; set; } = "localhost";

        public string LogTraceActive { get; set; } = "disable";
        public string LogTraceLevel { get; set; } = "Information";
        public string LogTraceServerUrl { get; set; } = "localhost";
        public string EnableOpenDocTime { get; set; } = "true";
        public string EnableSyncDocTime { get; set; } = "true";

        public string SocialsMain { get; set; } = "https://t.me/bim4everyone_group";
        public string SocialsNews { get; set; } = "https://t.me/bim4everyone_news";
        public string SocialsDiscuss { get; set; } = "https://t.me/bim4everyone_discuss";
        public string Socials2D { get; set; } = "https://t.me/bim4everyone_group/12";
        public string SocialsBim { get; set; } = "https://t.me/bim4everyone_group/11";
        public string SocialsAR { get; set; } = "https://t.me/bim4everyone_group/8";
        public string SocialsKR { get; set; } = "https://t.me/bim4everyone_group/7";
        public string SocialsOVVK { get; set; } = "https://t.me/bim4everyone_group/6";
        public string SocialsPage { get; set; } = "https://bim4everyone.com/";

        public string SocialsDownloads { get; set; } =
            "https://github.com/Bim4Everyone/Bim4EveryoneSetup/releases/latest";
    }
}