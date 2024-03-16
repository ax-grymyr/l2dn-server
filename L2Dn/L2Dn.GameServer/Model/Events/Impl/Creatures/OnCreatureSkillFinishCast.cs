using System.Runtime.CompilerServices;
using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * An instantly executed event when Caster has finished using a skill.
 * @author Nik
 */
public class OnCreatureSkillFinishCast: EventBase
{
	private Creature _caster;
	private WorldObject _target;
	private Skill _skill;
	private bool _simultaneously;
	
	public Creature getCaster()
	{
		return _caster;
	}
	
	public void setCaster(Creature caster)
	{
		_caster = caster;
	}
	
	public WorldObject getTarget()
	{
		return _target;
	}
	
	public void setTarget(WorldObject target)
	{
		_target = target;
	}
	
	public Skill getSkill()
	{
		return _skill;
	}
	
	public void setSkill(Skill skill)
	{
		_skill = skill;
	}
	
	public bool isSimultaneously()
	{
		return _simultaneously;
	}
	
	public void setSimultaneously(bool simultaneously)
	{
		_simultaneously = simultaneously;
	}
}