namespace BIM4EveryoneSetup;

internal sealed class MsiVariables {
    public static readonly string? BotAccessToken =
        Environment.GetEnvironmentVariable("B4E_BOT_ACCESS_TOKEN");

    public static readonly string? MsiVariablesFile =
        Environment.GetEnvironmentVariable("B4E_MSI_VARIABLES_PATH") ?? Constants.MsiVariablesFile;

    public string? AutoUpdate { get; set; }
    public string? RocketMode { get; set; }
    public string? CheckUpdates { get; set; }
    public string? UserCanExtend { get; set; }
    public string? UserCanConfig { get; set; }
    public string? CoreUserLocale { get; set; }

    public string? CorpName { get; set; }
    public string? CorpSettingsPath { get; set; }

    public string? TelemetryActive { get; set; }
    public string? TelemetryUseUTC { get; set; }
    public string? TelemetryServerUrl { get; set; }

    public string? AppTelemetryActive { get; set; }
    public string? AppTelemetryEventFlags { get; set; }
    public string? AppTelemetryServerUrl { get; set; }

    public string? LogTraceActive { get; set; }
    public string? LogTraceLevel { get; set; }
    public string? LogTraceServerUrl { get; set; }
    public string? EnableOpenDocTime { get; set; }
    public string? EnableSyncDocTime { get; set; }

    public string? SocialsMain { get; set; }
    public string? SocialsNews { get; set; }
    public string? SocialsDiscuss { get; set; }
    public string? Socials2D { get; set; }
    public string? SocialsBim { get; set; }
    public string? SocialsAR { get; set; }
    public string? SocialsKR { get; set; }
    public string? SocialsOVVK { get; set; }
    public string? SocialsPage { get; set; }

    public string? SocialsDownloads { get; set; }
}
