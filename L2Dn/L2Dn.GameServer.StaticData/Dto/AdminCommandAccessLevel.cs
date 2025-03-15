using L2Dn.GameServer.StaticData.Xml.AdminCommands;

namespace L2Dn.GameServer.Dto;

public sealed class AdminCommandAccessLevel
{
    private readonly int _accessLevel;

    internal AdminCommandAccessLevel(XmlAdminCommand command)
    {
        Command = command.Command;
        RequireConfirmation = command.ConfirmDialog;
        _accessLevel = command.AccessLevel;
    }

    internal AdminCommandAccessLevel(string command, bool confirmation, int accessLevel)
    {
        Command = command;
        RequireConfirmation = confirmation;
        _accessLevel = accessLevel;
    }

    /// <summary>
    /// The admin command.
    /// </summary>
    public string Command { get; }

    /// <summary>
    /// True if admin command requires confirmation before execution, false otherwise.
    /// </summary>
    public bool RequireConfirmation { get; }

    /// <summary>
    /// Return true if accessLevel is allowed to use the admin command which belongs to this access right,
    /// false otherwise.
    /// </summary>
    public bool HasAccess(int accessLevel) => accessLevel >= _accessLevel;
}