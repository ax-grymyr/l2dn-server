using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * Executed when the caster Creature tries to use a skill.
 * @author UnAfraid, Nik
 */
public class OnCreatureSkillUse: IBaseEvent
{
	private Creature _caster;
	private Skill _skill;
	private bool _simultaneously;
	
	public OnCreatureSkillUse()
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
		return EventType.ON_CREATURE_SKILL_USE;
	}
}