using System.Collections.Frozen;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.AdminCommands;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class AdminCommandData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AdminCommandData));

    private FrozenDictionary<string, AdminCommand> _adminCommands =
        FrozenDictionary<string, AdminCommand>.Empty;

    public static AdminCommandData Instance { get; } = new();

    private AdminCommandData()
    {
    }

    internal void Load()
    {
        XmlAdminCommandList document = XmlFileReader.LoadConfigXmlDocument<XmlAdminCommandList>("AdminCommands.xml");
        _adminCommands = document.Commands.
            Select(xmlAdminCommand => new AdminCommand(xmlAdminCommand)).
            ToFrozenDictionary(command => command.Command);

        _logger.Info($"{nameof(AdminCommandData)}: Loaded {_adminCommands.Count} access commands.");
    }

    public AdminCommand? GetAdminCommand(string command)
    {
        AdminCommand? adminCommand = _adminCommands.GetValueOrDefault(command);
        if (adminCommand is null)
            LogNoRightsForCommand(command);

        return adminCommand;
    }

    /// <summary>
    /// Returns true if the command can be used by the character with the access level.
    /// </summary>
    public bool HasAccess(string command, int accessLevel)
    {
        AdminCommand? adminCommand = _adminCommands.GetValueOrDefault(command);
        if (adminCommand == null)
        {
            if (accessLevel > 0 && accessLevel == AccessLevelData.Instance.HighestLevel)
            {
                adminCommand = new AdminCommand(command, true, accessLevel);
                Dictionary<string, AdminCommand> dict = _adminCommands.ToDictionary();
                dict[command] = adminCommand;
                _adminCommands = dict.ToFrozenDictionary();
                _logger.Info($"{nameof(AdminCommandData)}: No rights defined for admin command {command}; " +
                    $"auto setting access level {accessLevel}.");
            }
            else
            {
                LogNoRightsForCommand(command);
                return false;
            }
        }

        return adminCommand.HasAccess(accessLevel);
    }

    /// <summary>
    /// Returns true, if the command require confirmation, false otherwise.
    /// </summary>
    public bool RequireConfirmation(string command)
    {
        AdminCommand? adminCommand = _adminCommands.GetValueOrDefault(command);
        if (adminCommand == null)
        {
            LogNoRightsForCommand(command);
            return false;
        }

        return adminCommand.RequireConfirmation;
    }

    private static void LogNoRightsForCommand(string command)
    {
        _logger.Info($"{nameof(AdminCommandData)}: No rights defined for admin command {command}.");
    }
}