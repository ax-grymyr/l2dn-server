using System.Collections.Frozen;
using L2Dn.GameServer.StaticData.Xml.SecondaryAuth;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class SecondaryAuthData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(SecondaryAuthData));

    private FrozenSet<string> _forbiddenPasswords = FrozenSet<string>.Empty;
    private bool _enabled;
    private int _maxAttempts = 5;
    private TimeSpan _banTime = TimeSpan.FromMinutes(480);
    private string _recoveryLink = string.Empty;

    public static SecondaryAuthData Instance { get; } = new();

    private SecondaryAuthData()
    {
    }

    internal void Load()
    {
        XmlSecondaryAuth document = XmlFileReader.LoadConfigXmlDocument<XmlSecondaryAuth>("SecondaryAuth.xml");
        _enabled = document.Enabled;
        _maxAttempts = document.MaxAttempts;
        _banTime = TimeSpan.FromMinutes(document.BanTime);
        _recoveryLink = document.RecoveryLink;
        _forbiddenPasswords = document.ForbiddenPasswords.Distinct().ToFrozenSet();
        _logger.Info($"{nameof(SecondaryAuthData)}: Loaded {_forbiddenPasswords.Count} forbidden passwords.");
    }

    public bool Enabled => _enabled;

    public int MaxAttempts => _maxAttempts;

    public TimeSpan BanTime => _banTime;

    public string RecoveryLink => _recoveryLink;

    public FrozenSet<string> ForbiddenPasswords => _forbiddenPasswords;

    public bool IsForbiddenPassword(string password) => _forbiddenPasswords.Contains(password);
}