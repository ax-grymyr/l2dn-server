using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Items.Henna;

/**
 * @author Serenitty
 */
public class DyePotential
{
	private readonly int _id;
	private readonly int _slotId;
	private readonly int _skillId;
	private readonly Skill[] _skills;
	private readonly int _maxSkillLevel;

	public DyePotential(int id, int slotId, int skillId, int maxSkillLevel)
	{
		_id = id;
		_slotId = slotId;
		_skillId = skillId;
		_skills = new Skill[maxSkillLevel];
		for (int i = 1; i <= maxSkillLevel; i++)
		{
            // TODO pass skills as argument
            _skills[i - 1] = SkillData.Instance.GetSkill(skillId, i) ??
                throw new ArgumentException($"Skill id={skillId}, level={i} not found");
        }

		_maxSkillLevel = maxSkillLevel;
	}

	public int getId()
	{
		return _id;
	}

	public int getSlotId()
	{
		return _slotId;
	}

	public int getSkillId()
	{
		return _skillId;
	}

	public Skill getSkill(int level)
	{
		return _skills[level - 1];
	}

	public int getMaxSkillLevel()
	{
		return _maxSkillLevel;
	}
}