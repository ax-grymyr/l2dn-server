using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.AdminCommands;

public class XmlAdminCommand
{
    [XmlAttribute("command")]
    public string Command { get; set; } = string.Empty;

    [XmlAttribute("accessLevel")]
    public int AccessLevel { get; set; } = 7;

    [XmlAttribute("confirmDlg")]
    public bool ConfirmDialog { get; set; }
}