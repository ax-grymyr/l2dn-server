using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class AgathionData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AgathionData));
	
	private static readonly Map<int, AgathionSkillHolder> AGATHION_SKILLS = new();
	
	protected AgathionData()
	{
		load();
	}
	
	public void load()
	{
		AGATHION_SKILLS.clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "AgathionData.xml");
		document.Elements("list").Elements("agathion").ForEach(loadElement);
		LOGGER.Info(GetType().Name + ": Loaded " + AGATHION_SKILLS.size() + " agathion data.");
	}

	private void loadElement(XElement element)
	{
		int id = element.GetAttributeValueAsInt32("id");
		if (ItemData.getInstance().getTemplate(id) == null)
		{
			LOGGER.Info(GetType().Name + ": Could not find agathion with id " + id + ".");
			return;
		}

		int enchant = element.Attribute("enchant").GetInt32(0);

		Map<int, List<Skill>> mainSkills =
			AGATHION_SKILLS.containsKey(id) ? AGATHION_SKILLS.get(id).getMainSkills() : new();
		
		List<Skill> mainSkillList = new();
		String main = element.Attribute("mainSkill").GetString(string.Empty);
		foreach (String ids in main.Split(";"))
		{
			if (ids.isEmpty())
			{
				continue;
			}

			String[] split = ids.Split(",");
			int skillId = int.Parse(split[0]);
			int level = int.Parse(split[1]);

			Skill skill = SkillData.getInstance().getSkill(skillId, level);
			if (skill == null)
			{
				LOGGER.Info(GetType().Name + ": Could not find agathion skill id " + skillId + ".");
				return;
			}

			mainSkillList.add(skill);
		}

		mainSkills.put(enchant, mainSkillList);

		Map<int, List<Skill>> subSkills =
			AGATHION_SKILLS.containsKey(id) ? AGATHION_SKILLS.get(id).getSubSkills() : new();
		
		List<Skill> subSkillList = new();
		String sub = element.Attribute("subSkill").GetString(string.Empty);
		foreach (String ids in sub.Split(";"))
		{
			if (ids.isEmpty())
			{
				continue;
			}

			String[] split = ids.Split(",");
			int skillId = int.Parse(split[0]);
			int level = int.Parse(split[1]);

			Skill skill = SkillData.getInstance().getSkill(skillId, level);
			if (skill == null)
			{
				LOGGER.Info(GetType().Name + ": Could not find agathion skill id " + skillId + ".");
				return;
			}

			subSkillList.add(skill);
		}

		subSkills.put(enchant, subSkillList);

		AGATHION_SKILLS.put(id, new AgathionSkillHolder(mainSkills, subSkills));
	}

	public AgathionSkillHolder getSkills(int agathionId)
	{
		return AGATHION_SKILLS.get(agathionId);
	}
	
	public static AgathionData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly AgathionData INSTANCE = new AgathionData();
	}
}