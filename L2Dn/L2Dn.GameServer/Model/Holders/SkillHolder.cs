using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Holders;

/**
 * Simple class for storing skill id/level.
 * @author BiggBoss
 */
public class SkillHolder
{
	private readonly int _skillId;
	private readonly int _skillLevel;
	private readonly int _skillSubLevel;
	private Skill _skill;

	public SkillHolder(int skillId, int skillLevel)
	{
		_skillId = skillId;
		_skillLevel = skillLevel;
		_skillSubLevel = 0;
		_skill = null;
	}

	public SkillHolder(int skillId, int skillLevel, int skillSubLevel)
	{
		_skillId = skillId;
		_skillLevel = skillLevel;
		_skillSubLevel = skillSubLevel;
		_skill = null;
	}

	public SkillHolder(Skill skill)
	{
		_skillId = skill.getId();
		_skillLevel = skill.getLevel();
		_skillSubLevel = skill.getSubLevel();
		_skill = skill;
	}

	public int getSkillId()
	{
		return _skillId;
	}

	public int getSkillLevel()
	{
		return _skillLevel;
	}

	public int getSkillSubLevel()
	{
		return _skillSubLevel;
	}

	public Skill getSkill()
	{
		if (_skill == null)
		{
			_skill = SkillData.getInstance().getSkill(_skillId, Math.Max(_skillLevel, 1), _skillSubLevel);
		}

		return _skill;
	}

	public override bool Equals(Object? obj)
	{
		if (this == obj)
		{
			return true;
		}

		if (!(obj is SkillHolder))
		{
			return false;
		}

		SkillHolder holder = (SkillHolder)obj;
		return (holder.getSkillId() == _skillId) && (holder.getSkillLevel() == _skillLevel) &&
		       (holder.getSkillSubLevel() == _skillSubLevel);
	}

	public override int GetHashCode()
	{
		int prime = 31;
		int result = 1;
		result = (prime * result) + _skillId;
		result = (prime * result) + _skillLevel;
		result = (prime * result) + _skillSubLevel;
		return result;
	}

	public override String ToString()
	{
		return "[SkillId: " + _skillId + " Level: " + _skillLevel + "]";
	}
}