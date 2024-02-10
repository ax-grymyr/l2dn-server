using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Options;

/**
 * @author UnAfraid, Mobius
 */
public class OptionSkillHolder
{
	private readonly Skill _skill;
	private readonly double _chance;
	private readonly OptionSkillType _type;

	/**
	 * @param skill
	 * @param type
	 * @param chance
	 */
	public OptionSkillHolder(Skill skill, double chance, OptionSkillType type)
	{
		_skill = skill;
		_chance = chance;
		_type = type;
	}

	public Skill getSkill()
	{
		return _skill;
	}

	public double getChance()
	{
		return _chance;
	}

	public OptionSkillType getSkillType()
	{
		return _type;
	}
}