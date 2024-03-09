using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Relax effect implementation.
 */
public class Relax: AbstractEffect
{
	private readonly double _power;
	
	public Relax(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
		setTicks(@params.getInt("ticks"));
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.RELAXING.getMask();
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.RELAXING;
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isPlayer())
		{
			effected.getActingPlayer().sitDown(false);
		}
		else
		{
			effected.getAI().setIntention(CtrlIntention.AI_INTENTION_REST);
		}
	}
	
	public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isDead())
		{
			return false;
		}
		
		if (effected.isPlayer() && !effected.getActingPlayer().isSitting())
		{
			return false;
		}
		
		if (((effected.getCurrentHp() + 1) > effected.getMaxRecoverableHp()) && skill.isToggle())
		{
			effected.sendPacket(SystemMessageId.THAT_SKILL_HAS_BEEN_DE_ACTIVATED_AS_HP_WAS_FULLY_RECOVERED);
			return false;
		}
		
		double manaDam = _power * getTicksMultiplier();
		if ((manaDam > effected.getCurrentMp()) && skill.isToggle())
		{
			effected.sendPacket(SystemMessageId.YOUR_SKILL_WAS_DEACTIVATED_DUE_TO_LACK_OF_MP);
			return false;
		}
		
		effected.reduceCurrentMp(manaDam);
		
		return skill.isToggle();
	}
}