using System.Collections.Frozen;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.StaticData.Xml.CharacterClasses;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

/// <summary>
/// Loads the list of classes and it's info.
/// </summary>
public sealed class CharacterClassData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(CharacterClassData));

    private static FrozenDictionary<CharacterClass, CharacterClassInfo> _classData =
        FrozenDictionary<CharacterClass, CharacterClassInfo>.Empty;

    private CharacterClassData()
    {
    }

    public static CharacterClassData Instance { get; } = new();

    public void Load()
    {
        _classData = XmlLoader.LoadXmlDocument<XmlCharacterClassList>("stats/chars/classList.xml").Classes.Select(c =>
        {
            CharacterClass classId = (CharacterClass)c.ClassId;
            CharacterClass? parentClassId = c.ParentClassIdSpecified ? (CharacterClass)c.ParentClassId : null;
            return KeyValuePair.Create(classId, new CharacterClassInfo(classId, c.Name, parentClassId));
        }).ToFrozenDictionary();

        _logger.Info($"{nameof(CharacterClassData)}: Loaded {_classData.Count} class data.");
    }

    /// <summary>
    /// The complete class list.
    /// </summary>
    public FrozenDictionary<CharacterClass, CharacterClassInfo> ClassList => _classData;

    /// <summary>
    /// Gets the class info.
    /// </summary>
    public CharacterClassInfo GetClassInfo(CharacterClass classId) => _classData[classId];
}