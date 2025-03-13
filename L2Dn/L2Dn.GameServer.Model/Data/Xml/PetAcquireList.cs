using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Berezkin Nikolay
 */
public class PetAcquireList: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PetAcquireList));

	private readonly Map<int, List<PetSkillAcquireHolder>> _skills = new();

	protected PetAcquireList()
	{
		load();
	}

	public void load()
	{
		_skills.Clear();

		XDocument document = LoadXmlDocument(DataFileLocation.Data, "PetAcquireList.xml");
		document.Elements("list").Elements("pet").ForEach(parseElement);

		if (_skills.Count != 0)
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _skills.Count + " pet skills.");
		}
		else
		{
			LOGGER.Info(GetType().Name + ": System is disabled.");
		}
	}

	private void parseElement(XElement element)
	{
		int type = element.GetAttributeValueAsInt32("type");

		List<PetSkillAcquireHolder> list = new();
		element.Elements("skill").ForEach(el =>
		{
			int id = el.GetAttributeValueAsInt32("id");
			int lvl = el.GetAttributeValueAsInt32("lvl");
			int reqLvl = el.GetAttributeValueAsInt32("reqLvl");
			int evolve = el.GetAttributeValueAsInt32("evolve");
			int item = el.Attribute("item").GetInt32(-1);
			long itemAmount = el.Attribute("itemAmount").GetInt64(-1);

			list.Add(new PetSkillAcquireHolder(id, lvl, reqLvl, evolve, item < 0
				? null
				: new ItemHolder(item, itemAmount)));
		});

		_skills.put(type, list);
	}

	public List<PetSkillAcquireHolder>? getSkills(int type)
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