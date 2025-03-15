using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.SecondaryAuth;

[XmlRoot("list")]
public sealed class XmlSecondaryAuth
{
    [XmlElement("enabled")]
    public bool Enabled { get; set; }

    [XmlElement("maxAttempts")]
    public int MaxAttempts { get; set; }

    [XmlElement("banTime")]
    public int BanTime { get; set; }

    [XmlElement("recoveryLink")]
    public string RecoveryLink { get; set; } = string.Empty;

    [XmlArray("forbiddenPasswords")]
    [XmlArrayItem("password")]
    public List<string> ForbiddenPasswords { get; set; } = [];
}