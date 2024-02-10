using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * An instantly executed event when Caster has finished using a skill.
 * @author Nik
 */
public class OnCreatureSkillFinishCast: IBaseEvent
{
	private Creature _caster;
	private WorldObject _target;
	private Skill _skill;
	private bool _simultaneously;
	
	public OnCreatureSkillFinishCast()
	{
	}
	
	public Creature getCaster()
	{
		return _caster;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void setCaster(Creature caster)
	{
		_caster = caster;
	}
	
	public WorldObject getTarget()
	{
		return _target;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void setTarget(WorldObject target)
	{
		_target = target;
	}
	
	public Skill getSkill()
	{
		return _skill;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void setSkill(Skill skill)
	{
		_skill = skill;
	}
	
	public bool isSimultaneously()
	{
		return _simultaneously;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void setSimultaneously(bool simultaneously)
	{
		_simultaneously = simultaneously;
	}
	
	public EventType getType()
	{
		return EventType.ON_CREATURE_SKILL_FINISH_CAST;
	}
}