using L2Dn.GameServer.StaticData.Xml.AccessLevels;

namespace L2Dn.GameServer.Dto;

public sealed class AccessLevel
{
    private readonly Color _nameColor;
    private readonly Color _titleColor;

    internal AccessLevel(int accessLevel, string name, bool banned)
    {
        Level = accessLevel;
        Name = name;
        _nameColor = Colors.White;
        _titleColor = Colors.White;
        AllowTransaction = CanGiveDamage = CanGainExp = CanTakeAggro = !banned;
    }

    internal AccessLevel(XmlAccessLevel level)
    {
        Level = level.Level;
        Name = level.Name;

        if (!Color.TryParse(level.NameColor, out _nameColor))
            _nameColor = Colors.White;

        if (!Color.TryParse(level.TitleColor, out _titleColor))
            _titleColor = Colors.White;

        IsGM = level.IsGm;
        AllowPeaceAttack = level.AllowPeaceAttack;
        AllowFixedRes = level.AllowFixedRes;
        AllowTransaction = level.AllowTransaction;
        AllowAltG = level.AllowAltG;
        CanGiveDamage = level.GiveDamage;
        CanTakeAggro = level.TakeAggro;
        CanGainExp = level.GainExp;
    }

    /// <summary>
    /// The access level.
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// The access level name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The name color of the access level.
    /// </summary>
    public Color NameColor => _nameColor;

    /// <summary>
    /// The title color of the access level.
    /// </summary>
    public Color TitleColor => _titleColor;

    /// <summary>
    /// Returns if the access level has GM access or not.
    /// </summary>
    public bool IsGM { get; }

    /// <summary>
    /// Returns if the access level is allowed to attack in peace zone or not.
    /// </summary>
    public bool AllowPeaceAttack { get; }

    /// <summary>
    /// Returns if the access level is allowed to use fixed res or not.
    /// </summary>
    public bool AllowFixedRes { get; }

    /// <summary>
    /// Returns if the access level is allowed to perform transactions or not.
    /// </summary>
    public bool AllowTransaction { get; }

    /// <summary>
    /// Returns if the access level is allowed to use Alt-G commands or not.
    /// </summary>
    public bool AllowAltG { get; }

    /// <summary>
    /// Returns if the access level can give damage or not.
    /// </summary>
    public bool CanGiveDamage { get; }

    /// <summary>
    /// Returns if the access level can take aggro or not.
    /// </summary>
    public bool CanTakeAggro { get; }

    /// <summary>
    /// Returns if the access level can gain exp or not.
    /// </summary>
    public bool CanGainExp { get; }
}