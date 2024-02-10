using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class AgathionData
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
		parseDatapackFile("data/AgathionData.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + AGATHION_SKILLS.size() + " agathion data.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "agathion", agathionNode =>
		{
			StatSet set = new StatSet(parseAttributes(agathionNode));
			
			int id = set.getInt("id");
			if (ItemData.getInstance().getTemplate(id) == null)
			{
				LOGGER.Info(GetType().Name + ": Could not find agathion with id " + id + ".");
				return;
			}
			
			int enchant = set.getInt("enchant", 0);
			
			Map<int, List<Skill>> mainSkills = AGATHION_SKILLS.containsKey(id) ? AGATHION_SKILLS.get(id).getMainSkills() : new();
			List<Skill> mainSkillList = new();
			String main = set.getString("mainSkill", "");
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
			
			Map<int, List<Skill>> subSkills = AGATHION_SKILLS.containsKey(id) ? AGATHION_SKILLS.get(id).getSubSkills() : new();
			List<Skill> subSkillList = new();
			String sub = set.getString("subSkill", "");
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
		}));
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