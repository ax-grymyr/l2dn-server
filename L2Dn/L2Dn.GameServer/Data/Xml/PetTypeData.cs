using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class PetTypeData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PetTypeData));
	
	private readonly Map<int, SkillHolder> _skills = new();
	private readonly Map<int, String> _names = new();
	
	protected PetTypeData()
	{
		load();
	}
	
	public void load()
	{
		_skills.clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "PetTypes.xml");
		document.Elements("list").Elements("pet").ForEach(parseElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _skills.size() + " pet types.");
	}

	private void parseElement(XElement element)
	{
		int id = element.Attribute("id").GetInt32();
		int skillId = element.Attribute("skillId").GetInt32(0);
		int skillLvl = element.Attribute("skillLvl").GetInt32(0);
		string name = element.Attribute("name").GetString();
		_skills.put(id, new SkillHolder(skillId, skillLvl));
		_names.put(id, name);
	}

	public SkillHolder getSkillByName(String name)
	{
		foreach (var entry in _names)
		{
			if (name.startsWith(entry.Value))
			{
				return _skills.get(entry.Key);
			}
		}
		return null;
	}
	
	public int getIdByName(String name)
	{
		foreach (var entry in _names)
		{
			if (name.endsWith(entry.Value))
			{
				return entry.Key;
			}
		}
		return 0;
	}
	
	public String getNamePrefix(int id)
	{
		return _names.get(id);
	}
	
	public String getRandomName()
	{
		return _names.Where(e => e.Key > 100).First().Value;
	}
	
	public KeyValuePair<int, SkillHolder> getRandomSkill()
	{
		return _skills.Where(e => e.Value.getSkillId() > 0).First();
	}
	
	public static PetTypeData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly PetTypeData INSTANCE = new();
	}
}