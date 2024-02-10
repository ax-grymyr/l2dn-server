using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author UnAfraid
 */
public class PlayerSkillHolder: ISkillsHolder
{
	private readonly Map<int, Skill> _skills = new();

	public PlayerSkillHolder(Player player)
	{
		foreach (Skill skill in player.getSkills().values())
		{
			// Adding only skills that can be learned by the player.
			if (SkillTreeData.getInstance().isSkillAllowed(player, skill))
			{
				addSkill(skill);
			}
		}
	}

	/**
	 * @return the map containing this character skills.
	 */
	public Map<int, Skill> getSkills()
	{
		return _skills;
	}

	/**
	 * Add a skill to the skills map.
	 * @param skill
	 */
	public Skill addSkill(Skill skill)
	{
		return _skills.put(skill.getId(), skill);
	}

	/**
	 * Return the level of a skill owned by the Creature.
	 * @param skillId The identifier of the Skill whose level must be returned
	 * @return The level of the Skill identified by skillId
	 */
	public int getSkillLevel(int skillId)
	{
		Skill skill = getKnownSkill(skillId);
		return (skill == null) ? 0 : skill.getLevel();
	}

	/**
	 * @param skillId The identifier of the Skill to check the knowledge
	 * @return the skill from the known skill.
	 */
	public Skill getKnownSkill(int skillId)
	{
		return _skills.get(skillId);
	}

	public Skill removeSkill(Skill skill)
	{
		return _skills.remove(skill.getId());
	}
}