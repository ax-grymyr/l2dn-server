namespace L2Dn.AuthServer.Configuration;

public sealed class SettingsConfig
{
    public bool AutoCreateAccounts { get; set; }
    public bool ShowLicense { get; set; } = true;
    public bool ShowPIAgreement { get; set; }
}