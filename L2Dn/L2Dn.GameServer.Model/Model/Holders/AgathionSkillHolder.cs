using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public sealed class AgathionSkillHolder
{
	private readonly FrozenDictionary<int, ImmutableArray<Skill>> _mainSkill;
	private readonly FrozenDictionary<int, ImmutableArray<Skill>> _subSkill;

	public AgathionSkillHolder(int agathionItemId, FrozenDictionary<int, ImmutableArray<Skill>> mainSkill,
		FrozenDictionary<int, ImmutableArray<Skill>> subSkill)
	{
		ItemId = agathionItemId;
		_mainSkill = mainSkill;
		_subSkill = subSkill;
	}

	public int ItemId { get; }

	public FrozenDictionary<int, ImmutableArray<Skill>> getMainSkills()
	{
		return _mainSkill;
	}

	public FrozenDictionary<int, ImmutableArray<Skill>> getSubSkills()
	{
		return _subSkill;
	}

	public ImmutableArray<Skill> getMainSkills(int enchantLevel)
	{
		return _mainSkill.TryGetValue(enchantLevel, out ImmutableArray<Skill> skills)
			? skills
			: ImmutableArray<Skill>.Empty;
	}

	public ImmutableArray<Skill> getSubSkills(int enchantLevel)
	{
		return _subSkill.TryGetValue(enchantLevel, out ImmutableArray<Skill> skills)
			? skills
			: ImmutableArray<Skill>.Empty;
	}
}