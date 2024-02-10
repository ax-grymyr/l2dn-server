using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class PetTypeData
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
		parseDatapackFile("data/PetTypes.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _skills.size() + " pet types.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "pet", petNode =>
		{
			StatSet set = new StatSet(parseAttributes(petNode));
			int id = set.getInt("id");
			_skills.put(id, new SkillHolder(set.getInt("skillId", 0), set.getInt("skillLvl", 0)));
			_names.put(id, set.getString("name"));
		}));
	}
	
	public SkillHolder getSkillByName(String name)
	{
		foreach (Entry<int, String> entry in _names.entrySet())
		{
			if (name.startsWith(entry.getValue()))
			{
				return _skills.get(entry.getKey());
			}
		}
		return null;
	}
	
	public int getIdByName(String name)
	{
		foreach (Entry<int, String> entry in _names.entrySet())
		{
			if (name.endsWith(entry.getValue()))
			{
				return entry.getKey();
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
		return _names.entrySet().stream().filter(e => e.getKey() > 100).findAny().get().getValue();
	}
	
	public Entry<int, SkillHolder> getRandomSkill()
	{
		return _skills.entrySet().stream().filter(e => e.getValue().getSkillId() > 0).findAny().get();
	}
	
	public static PetTypeData getInstance()
	{
		return PetTypeData.SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly PetTypeData INSTANCE = new();
	}
}