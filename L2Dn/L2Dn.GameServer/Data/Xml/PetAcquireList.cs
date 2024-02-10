using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Berezkin Nikolay
 */
public class PetAcquireList
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PetAcquireList));
	
	private readonly Map<int, List<PetSkillAcquireHolder>> _skills = new();
	
	protected PetAcquireList()
	{
		load();
	}
	
	public void load()
	{
		_skills.clear();
		parseDatapackFile("data/PetAcquireList.xml");
		
		if (!_skills.isEmpty())
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _skills.size() + " pet skills.");
		}
		else
		{
			LOGGER.Info(GetType().Name + ": System is disabled.");
		}
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("pet".equalsIgnoreCase(d.getNodeName()))
					{
						NamedNodeMap attrs = d.getAttributes();
						Node att;
						StatSet set = new StatSet();
						for (int i = 0; i < attrs.getLength(); i++)
						{
							att = attrs.item(i);
							set.set(att.getNodeName(), att.getNodeValue());
						}
						
						int type = parseInteger(attrs, "type");
						List<PetSkillAcquireHolder> list = new();
						for (Node b = d.getFirstChild(); b != null; b = b.getNextSibling())
						{
							attrs = b.getAttributes();
							if ("skill".equalsIgnoreCase(b.getNodeName()))
							{
								list.add(new PetSkillAcquireHolder(parseInteger(attrs, "id"), parseInteger(attrs, "lvl"), parseInteger(attrs, "reqLvl"), parseInteger(attrs, "evolve"), parseInteger(attrs, "item") == null ? null : new ItemHolder(parseInteger(attrs, "item"), Parse(attrs, "itemAmount"))));
							}
						}
						
						_skills.put(type, list);
					}
				}
			}
		}
	}
	
	public List<PetSkillAcquireHolder> getSkills(int type)
	{
		return _skills.get(type);
	}
	
	public Map<int, List<PetSkillAcquireHolder>> getAllSkills()
	{
		return _skills;
	}
	
	public int getSpecialSkillByType(int petType)
	{
		switch (petType)
		{
			case 15:
			{
				return 49001;
			}
			case 14:
			{
				return 49011;
			}
			case 12:
			{
				return 49021;
			}
			case 13:
			{
				return 49031;
			}
			case 17:
			{
				return 49041;
			}
			case 16:
			{
				return 49051;
			}
			default:
			{
				throw new InvalidOperationException("Unexpected value: " + petType);
			}
		}
	}
	
	public static PetAcquireList getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly PetAcquireList INSTANCE = new();
	}
}