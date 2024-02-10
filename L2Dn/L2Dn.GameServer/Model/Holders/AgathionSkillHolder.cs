using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class AgathionSkillHolder
{
	private readonly Map<int, List<Skill>> _mainSkill;
	private readonly Map<int, List<Skill>> _subSkill;

	public AgathionSkillHolder(Map<int, List<Skill>> mainSkill, Map<int, List<Skill>> subSkill)
	{
		_mainSkill = mainSkill;
		_subSkill = subSkill;
	}

	public Map<int, List<Skill>> getMainSkills()
	{
		return _mainSkill;
	}

	public Map<int, List<Skill>> getSubSkills()
	{
		return _subSkill;
	}

	public List<Skill> getMainSkills(int enchantLevel)
	{
		if (!_mainSkill.containsKey(enchantLevel))
		{
			return Collections.emptyList();
		}

		return _mainSkill.get(enchantLevel);
	}

	public List<Skill> getSubSkills(int enchantLevel)
	{
		if (!_subSkill.containsKey(enchantLevel))
		{
			return Collections.emptyList();
		}

		return _subSkill.get(enchantLevel);
	}
}