using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Chameleon Rest effect implementation.
 */
public class ChameleonRest: AbstractEffect
{
	private readonly double _power;
	
	public ChameleonRest(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
		setTicks(@params.getInt("ticks"));
	}
	
	public override long getEffectFlags()
	{
		return (EffectFlag.SILENT_MOVE.getMask() | EffectFlag.RELAXING.getMask());
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.RELAXING;
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
		
		double manaDam = _power * getTicksMultiplier();
		if (manaDam > effected.getCurrentMp())
		{
			effected.sendPacket(SystemMessageId.YOUR_SKILL_WAS_DEACTIVATED_DUE_TO_LACK_OF_MP);
			return false;
		}
		
		effected.reduceCurrentMp(manaDam);
		return skill.isToggle();
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
}