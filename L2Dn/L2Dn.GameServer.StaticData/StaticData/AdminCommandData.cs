using System.Collections.Frozen;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.AdminCommands;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class AdminCommandData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AdminCommandData));

    private FrozenDictionary<string, AdminCommandAccessLevel> _adminCommandAccessLevels =
        FrozenDictionary<string, AdminCommandAccessLevel>.Empty;

    public static AdminCommandData Instance { get; } = new();

    private AdminCommandData()
    {
    }

    internal void Load()
    {
        XmlAdminCommandList document = XmlFileReader.LoadConfigXmlDocument<XmlAdminCommandList>("AdminCommands.xml");
        _adminCommandAccessLevels = document.Commands.
            Select(xmlAdminCommand => new AdminCommandAccessLevel(xmlAdminCommand)).
            ToFrozenDictionary(command => command.Command);

        _logger.Info($"{nameof(AdminCommandData)}: Loaded {_adminCommandAccessLevels.Count} access commands.");
    }

    /// <summary>
    /// Returns true if the command can be used by the character with the access level.
    /// </summary>
    public bool HasAccess(string command, int accessLevel)
    {
        AdminCommandAccessLevel? adminCommand = _adminCommandAccessLevels.GetValueOrDefault(command);
        if (adminCommand == null)
        {
            if (accessLevel > 0 && accessLevel == AccessLevelData.Instance.HighestLevel)
            {
                adminCommand = new AdminCommandAccessLevel(command, true, accessLevel);
                Dictionary<string, AdminCommandAccessLevel> dict = _adminCommandAccessLevels.ToDictionary();
                dict[command] = adminCommand;
                _adminCommandAccessLevels = dict.ToFrozenDictionary();
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
        AdminCommandAccessLevel? adminCommandAccessLevel = _adminCommandAccessLevels.GetValueOrDefault(command);
        if (adminCommandAccessLevel == null)
        {
            LogNoRightsForCommand(command);
            return false;
        }

        return adminCommandAccessLevel.RequireConfirmation;
    }

    private static void LogNoRightsForCommand(string command)
    {
        _logger.Info($"{nameof(AdminCommandData)}: No rights defined for admin command {command}.");
    }
}