using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Mana Heal By Level effect implementation.
 * @author UnAfraid
 */
public class ManaHealByLevel: AbstractEffect
{
	private readonly double _power;
	
	public ManaHealByLevel(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.MANAHEAL_BY_LEVEL;
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isDead() || effected.isDoor() || effected.isMpBlocked())
		{
			return;
		}
		
		if ((effected != effector) && effected.isAffected(EffectFlag.FACEOFF))
		{
			return;
		}
		
		double amount = _power;
		
		// recharged mp influenced by difference between target level and skill level
		// if target is within 5 levels or lower then skill level there's no penalty.
		amount = effected.getStat().getValue(Stat.MANA_CHARGE, amount);
		if (effected.getLevel() > skill.getMagicLevel())
		{
			int levelDiff = effected.getLevel() - skill.getMagicLevel();
			// if target is too high compared to skill level, the amount of recharged mp gradually decreases.
			if (levelDiff == 6)
			{
				amount *= 0.9; // only 90% effective
			}
			else if (levelDiff == 7)
			{
				amount *= 0.8; // 80%
			}
			else if (levelDiff == 8)
			{
				amount *= 0.7; // 70%
			}
			else if (levelDiff == 9)
			{
				amount *= 0.6; // 60%
			}
			else if (levelDiff == 10)
			{
				amount *= 0.5; // 50%
			}
			else if (levelDiff == 11)
			{
				amount *= 0.4; // 40%
			}
			else if (levelDiff == 12)
			{
				amount *= 0.3; // 30%
			}
			else if (levelDiff == 13)
			{
				amount *= 0.2; // 20%
			}
			else if (levelDiff == 14)
			{
				amount *= 0.1; // 10%
			}
			else if (levelDiff >= 15)
			{
				amount = 0; // 0mp recharged
			}
		}
		
		// Prevents overheal and negative amount
		amount = Math.Max(Math.Min(amount, effected.getMaxRecoverableMp() - effected.getCurrentMp()), 0);
		if (amount != 0)
		{
			double newMp = amount + effected.getCurrentMp();
			effected.setCurrentMp(newMp, false);
			effected.broadcastStatusUpdate(effector);
		}
		
		SystemMessagePacket sm = new SystemMessagePacket(effector.getObjectId() != effected.getObjectId() ? SystemMessageId.YOU_HAVE_RECOVERED_S2_MP_WITH_C1_S_HELP : SystemMessageId.S1_MP_HAS_BEEN_RESTORED);
		if (effector.getObjectId() != effected.getObjectId())
		{
			sm.Params.addString(effector.getName());
		}
		sm.Params.addInt((int) amount);
		effected.sendPacket(sm);
	}
}