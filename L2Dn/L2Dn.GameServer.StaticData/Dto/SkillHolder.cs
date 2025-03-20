using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Holders;

/// <summary>
/// Simple class for storing skill id/level.
/// </summary>
public class SkillHolder: IEquatable<SkillHolder> // TODO: this class must never exist, instead directly use Skill
{
	private readonly int _skillId;
	private readonly int _skillLevel;
	private readonly int _skillSubLevel;
	private Skill? _skill;

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
		_skillId = skill.Id;
		_skillLevel = skill.Level;
		_skillSubLevel = skill.SubLevel;
		_skill = skill;
	}

	public int getSkillId() => _skillId;
	public int getSkillLevel() => _skillLevel;
	public int getSkillSubLevel() => _skillSubLevel;

    public Skill getSkill() =>
        _skill ??= SkillData.Instance.GetSkill(_skillId, Math.Max(_skillLevel, 1), _skillSubLevel) ??
            throw new InvalidOperationException(
                $"Skill id={_skillId}, level={_skillLevel}, subLevel={_skillSubLevel} not found!");

	public bool Equals(SkillHolder? other)
	{
		if (ReferenceEquals(this, other))
			return true;

		if (ReferenceEquals(other, null))
			return false;

		return other._skillId == _skillId && other._skillLevel == _skillLevel && other._skillSubLevel == _skillSubLevel;
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(this, obj))
			return true;

		if (ReferenceEquals(obj, null))
			return false;

		return obj is SkillHolder other && other._skillId == _skillId && other._skillLevel == _skillLevel &&
			other._skillSubLevel == _skillSubLevel;
	}

	public override int GetHashCode() => HashCode.Combine(_skillId, _skillLevel, _skillSubLevel);

	public override string ToString() => $"[SkillId: {_skillId} Level: {_skillLevel}]";
}