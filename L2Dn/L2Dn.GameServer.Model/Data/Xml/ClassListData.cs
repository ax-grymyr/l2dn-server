using System.Collections.Frozen;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/// <summary>
/// Loads the list of classes and it's info.
/// </summary>
public sealed class ClassListData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(ClassListData));

	private static FrozenDictionary<CharacterClass, ClassInfoHolder> _classData =
		FrozenDictionary<CharacterClass, ClassInfoHolder>.Empty;
	
	/**
	 * Instantiates a new class list data.
	 */
	private ClassListData()
	{
		load();
	}
	
	public void load()
	{
		_classData = LoadXmlDocument<XmlCharacterClassList>(DataFileLocation.Data, "stats/chars/classList.xml")
			.Classes.Select(c =>
			{
				CharacterClass classId = (CharacterClass)c.ClassId;
				string className = c.Name;
				CharacterClass? parentClassId = c.ParentClassIdSpecified ? (CharacterClass)c.ParentClassId : null;
				return (Key: classId, Value: new ClassInfoHolder(classId, className, parentClassId));
			}).ToFrozenDictionary(t => t.Key, t => t.Value);
		
		_logger.Info(GetType().Name + ": Loaded " + _classData.Count + " class data.");
	}
	
	/**
	 * Gets the class list.
	 * @return the complete class list.
	 */
	public FrozenDictionary<CharacterClass, ClassInfoHolder> getClassList()
	{
		return _classData;
	}
	
	/**
	 * Gets the class info.
	 * @param classId the class Id as integer.
	 * @return the class info related to the given {@code classId}.
	 */
	public ClassInfoHolder getClass(CharacterClass classId)
	{
		return _classData[classId];
	}
	
	/**
	 * Gets the single instance of ClassListData.
	 * @return single instance of ClassListData
	 */
	public static ClassListData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ClassListData INSTANCE = new();
	}
}