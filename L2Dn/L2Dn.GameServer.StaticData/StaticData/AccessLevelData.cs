using System.Collections.Frozen;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.AccessLevels;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class AccessLevelData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AccessLevelData));

    private FrozenDictionary<int, AccessLevel> _accessLevels = FrozenDictionary<int, AccessLevel>.Empty;
    private AccessLevel _defaultAccessLevel = new(-1, "Banned", true);
    private AccessLevel _bannedAccessLevel = new(0, "User", false);
    private AccessLevel _masterAccessLevel = new(100, "Master", false);
    private int _highestLevel;

    public static AccessLevelData Instance { get; } = new();

    private AccessLevelData()
    {
    }

    public void Load()
    {
        XmlAccessLevelList document = XmlLoader.LoadConfigXmlDocument<XmlAccessLevelList>("AccessLevels.xml");

        _accessLevels = document.AccessLevels.Select(xmlAccessLevel => new AccessLevel(xmlAccessLevel)).
            ToFrozenDictionary(level => level.Level);

        _highestLevel = _accessLevels.Count == 0 ? 0 : _accessLevels.Keys.Max();

        if (!_accessLevels.TryGetValue(-1, out AccessLevel? bannedAccessLevel) || !_accessLevels.ContainsKey(0) ||
            _highestLevel < 100)
        {
            _logger.Error($"{nameof(AccessLevelData)}: At least 3 access levels must be defined, " +
                "-1 for banned characters, 0 for regular characters and 100 for GMs.");

            throw new InvalidOperationException("Invalid access level data.");
        }

        _bannedAccessLevel = bannedAccessLevel;
        _defaultAccessLevel = GetAccessLevel(Config.General.DEFAULT_ACCESS_LEVEL);
        _masterAccessLevel = GetAccessLevel(_highestLevel);

        _logger.Info($"{nameof(AccessLevelData)}: Loaded {_accessLevels.Count} access levels.");
    }

    public int HighestLevel => _highestLevel;

    /// <summary>
    /// Gets the default access level.
    /// </summary>
    public AccessLevel DefaultAccessLevel => _defaultAccessLevel;

    /// <summary>
    /// Gets the master access level.
    /// </summary>
    public AccessLevel MasterAccessLevel => _masterAccessLevel;

    /// <summary>
    /// Returns the access level by level number.
    /// </summary>
    public AccessLevel GetAccessLevel(int accessLevelNum)
    {
        if (accessLevelNum < 0)
            return _bannedAccessLevel;

        if (_accessLevels.TryGetValue(accessLevelNum, out AccessLevel? accessLevel))
            return accessLevel;

        return _accessLevels.OrderByDescending(kvp => kvp.Key).First(kvp => kvp.Key <= accessLevelNum).Value;
    }
}