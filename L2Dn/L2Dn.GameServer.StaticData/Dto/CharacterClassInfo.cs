using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Dto;

/// <summary>
/// This class holds the information of the player class.
/// </summary>
/// <param name="Class"></param>
/// <param name="Name">The hardcoded in-game class name.</param>
/// <param name="ParentClass">The parent class id.</param>
public sealed record CharacterClassInfo(CharacterClass Class, string Name, CharacterClass? ParentClass)
{
    /// <summary>
    /// The class client Id formatted to be displayed on an HTML.
    /// </summary>
    public string GetClientCode()
    {
        // TODO: Verify client ids above.
        // return "&$" + getClassClientId() + ";";
        return Name;
    }

    /// <summary>
    /// The class client id.
    /// </summary>
    private int GetClassClientId()
    {
        int classClientId = (int)Class;
        return classClientId switch
        {
            >= 0 and <= 57 => classClientId + 247,
            >= 88 and <= 118 => classClientId + 1071,
            >= 123 and <= 136 => classClientId + 1438,
            >= 139 and <= 146 => classClientId + 2338,
            >= 148 and <= 181 => classClientId + 2884,
            >= 182 and <= 189 => classClientId + 3121,
            >= 192 and <= 211 => classClientId + 12817, // TODO: Find proper ids.
            _ => classClientId,
        };
    }
}