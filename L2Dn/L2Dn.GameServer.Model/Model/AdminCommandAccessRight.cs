using L2Dn.GameServer.Data.Xml;
using L2Dn.Model.DataPack;

namespace L2Dn.GameServer.Model;

public class AdminCommandAccessRight
{
    private readonly string _adminCommand;
    private readonly int _accessLevel;
    private readonly bool _requireConfirm;

    public AdminCommandAccessRight(XmlAdminCommand command)
    {
        _adminCommand = command.Command;
        _requireConfirm = command.ConfirmDialog;
        _accessLevel = command.AccessLevel;
    }

    public AdminCommandAccessRight(string command, bool confirm, int level)
    {
        _adminCommand = command;
        _requireConfirm = confirm;
        _accessLevel = level;
    }

    /**
     * @return the admin command the access right belongs to
     */
    public string getAdminCommand()
    {
        return _adminCommand;
    }

    /**
     * @param characterAccessLevel
     * @return {@code true} if characterAccessLevel is allowed to use the admin command which belongs to this access right, {@code false} otherwise
     */
    public bool hasAccess(AccessLevel characterAccessLevel)
    {
        AccessLevel? accessLevel = AdminData.getInstance().getAccessLevel(_accessLevel);
        return accessLevel != null && (accessLevel.getLevel() == characterAccessLevel.getLevel() ||
                                       characterAccessLevel.hasChildAccess(accessLevel));
    }

    /**
     * @return {@code true} if admin command requires confirmation before execution, {@code false} otherwise.
     */
    public bool getRequireConfirm()
    {
        return _requireConfirm;
    }
}