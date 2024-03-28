using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Base;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures;

/**
 * Executed when the caster Creature tries to use a skill.
 * @author UnAfraid, Nik
 */
public class OnCreatureSkillUse: TerminateEventBase
{
	private Creature _caster;
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