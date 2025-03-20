using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Events.Impl.Npcs;

/**
 * @author UnAfraid
 */
public class OnNpcSkillSee: EventBase
{
	private readonly Npc _npc;
	private readonly Player _caster;
	private readonly Skill _skill;
	private readonly WorldObject[] _targets;
	private readonly bool _isSummon;

	public OnNpcSkillSee(Npc npc, Player caster, Skill skill, bool isSummon, params WorldObject[] targets)
	{
		_npc = npc;
		_caster = caster;
		_skill = skill;
		_isSummon = isSummon;
		_targets = targets;
	}

	public Npc getTarget()
	{
		return _npc;
	}

	public Player getCaster()
	{
		return _caster;
	}

	public Skill getSkill()
	{
		return _skill;
	}

	public WorldObject[] getTargets()
	{
		return _targets;
	}

	public bool isSummon()
	{
		return _isSummon;
	}
}