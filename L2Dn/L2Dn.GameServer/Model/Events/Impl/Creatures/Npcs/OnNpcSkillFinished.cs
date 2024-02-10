using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;

/**
 * @author UnAfraid
 */
public class OnNpcSkillFinished: IBaseEvent
{
	private readonly Npc _caster;
	private readonly Player _target;
	private readonly Skill _skill;
	
	public OnNpcSkillFinished(Npc caster, Player target, Skill skill)
	{
		_caster = caster;
		_target = target;
		_skill = skill;
	}
	
	public Player getTarget()
	{
		return _target;
	}
	
	public Npc getCaster()
	{
		return _caster;
	}
	
	public Skill getSkill()
	{
		return _skill;
	}
	
	public EventType getType()
	{
		return EventType.ON_NPC_SKILL_FINISHED;
	}
}