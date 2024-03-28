using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Skills;

/**
 * @author Mobius
 */
public class AmmunitionSkillList
{
	private static readonly Set<int> SKILLS = new();
	
	public static void add(List<ItemSkillHolder> skills)
	{
		foreach (ItemSkillHolder skill in skills)
		{
			SKILLS.add(skill.getSkillId());
		}
	}
	
	public static Set<int> values()
	{
		return SKILLS;
	}
}