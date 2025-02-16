using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Heal Percent effect implementation.
 * @author UnAfraid
 */
public class HealPercent: AbstractEffect
{
	private readonly int _power;
	
	public HealPercent(StatSet @params)
	{
		_power = @params.getInt("power", 0);
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.HEAL;
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isDead() || effected.isDoor() || effected.isHpBlocked())
		{
			return;
		}
		
		double amount = 0;
		double power = _power;
		bool full = (power == 100.0);
		
		amount = full ? effected.getMaxHp() : (effected.getMaxHp() * power) / 100.0;
		if ((item != null) && (item.isPotion() || item.isElixir()))
		{
			amount += effected.getStat().getValue(Stat.ADDITIONAL_POTION_HP, 0);
			
			// Classic Potion Mastery
			// TODO: Create an effect if more mastery skills are added.
			amount *= 1 + (effected.getAffectedSkillLevel((int)CommonSkill.POTION_MASTERY) / 100.0);
		}
		
		// Prevents overheal
		amount = Math.Min(amount, Math.Max(0, effected.getMaxRecoverableHp() - effected.getCurrentHp()));
		if (amount >= 0)
		{
			if (amount != 0)
			{
				double newHp = amount + effected.getCurrentHp();
				effected.setCurrentHp(newHp, false);
				effected.broadcastStatusUpdate(effector);
			}
			
			SystemMessagePacket sm;
			if (effector.ObjectId != effected.ObjectId)
			{
				sm = new SystemMessagePacket(SystemMessageId.S2_HP_HAS_BEEN_RECOVERED_BY_C1);
				sm.Params.addString(effector.getName());
			}
			else
			{
				sm = new SystemMessagePacket(SystemMessageId.YOU_VE_RECOVERED_S1_HP);
			}
			sm.Params.addInt((int) amount);
			effected.sendPacket(sm);
		}
		else
		{
			double damage = -amount;
			effected.reduceCurrentHp(damage, effector, skill, false, false, false, false);
			effector.sendDamageMessage(effected, skill, (int) damage, 0, false, false, false);
		}
	}
}
