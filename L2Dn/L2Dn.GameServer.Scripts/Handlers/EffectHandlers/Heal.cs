using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Heal effect implementation.
 * @author UnAfraid
 */
public class Heal: AbstractEffect
{
	private readonly double _power;
	
	public Heal(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
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
		
		if ((effected != effector) && effected.isAffected(EffectFlag.FACEOFF))
		{
			return;
		}
		
		double amount = _power;
		if ((item != null) && (item.isPotion() || item.isElixir()))
		{
			amount += effected.getStat().getValue(Stat.ADDITIONAL_POTION_HP, 0);
			
			// Classic Potion Mastery
			// TODO: Create an effect if more mastery skills are added.
			amount *= 1 + (effected.getAffectedSkillLevel((int)CommonSkill.POTION_MASTERY) / 100);
		}
		
		double staticShotBonus = 0;
		double mAtkMul = 1;
		bool sps = skill.isMagic() && effector.isChargedShot(ShotType.SPIRITSHOTS);
		bool bss = skill.isMagic() && effector.isChargedShot(ShotType.BLESSED_SPIRITSHOTS);
		double shotsBonus = effector.getStat().getValue(Stat.SHOTS_BONUS);
		if (((sps || bss) && (effector.isPlayer() && effector.getActingPlayer().isMageClass())) || effector.isSummon())
		{
			staticShotBonus = skill.getMpConsume(); // static bonus for spiritshots
			mAtkMul = bss ? 4 * shotsBonus : 2 * shotsBonus;
			staticShotBonus *= bss ? 2.4 : 1.0;
		}
		else if ((sps || bss) && effector.isNpc())
		{
			staticShotBonus = 2.4 * skill.getMpConsume(); // always blessed spiritshots
			mAtkMul = 4 * shotsBonus;
		}
		else
		{
			// no static bonus
			// grade dynamic bonus
			Item weaponInst = effector.getActiveWeaponInstance();
			if (weaponInst != null)
			{
				mAtkMul = weaponInst.getTemplate().getCrystalType() == CrystalType.S84 ? 4 : weaponInst.getTemplate().getCrystalType() == CrystalType.S80 ? 2 : 1;
			}
			// shot dynamic bonus
			mAtkMul = bss ? mAtkMul * 4 : mAtkMul + 1;
		}
		
		if (!skill.isStatic())
		{
			amount += staticShotBonus + Math.Sqrt(mAtkMul * effector.getMAtk());
			amount *= effected.getStat().getValue(Stat.HEAL_EFFECT, 1);
			amount += effected.getStat().getValue(Stat.HEAL_EFFECT_ADD, 0);
			amount *= (item == null) && effector.isPlayable() ? Config.PLAYER_HEALING_SKILL_MULTIPLIERS.GetValueOrDefault(effector.getActingPlayer().getClassId(), 1f) : 1f;
			// Heal critic, since CT2.3 Gracia Final
			if (skill.isMagic() && Formulas.calcCrit(skill.getMagicCriticalRate(), effector, effected, skill))
			{
				amount *= 3;
				effector.sendPacket(SystemMessageId.M_CRITICAL);
				effector.sendPacket(new ExMagicAttackInfoPacket(effector.ObjectId, effected.ObjectId, ExMagicAttackInfoPacket.CRITICAL_HEAL));
				if (effected.isPlayer() && (effected != effector))
				{
					effected.sendPacket(new ExMagicAttackInfoPacket(effector.ObjectId, effected.ObjectId, ExMagicAttackInfoPacket.CRITICAL_HEAL));
				}
			}
		}
		
		// Prevents overheal
		amount = Math.Min(amount, Math.Max(0, effected.getMaxRecoverableHp() - effected.getCurrentHp()));
		if (amount != 0)
		{
			double newHp = amount + effected.getCurrentHp();
			effected.setCurrentHp(newHp, false);
			effected.broadcastStatusUpdate(effector);
		}
		
		if (effected.isPlayer())
		{
			if (skill.getId() == 4051)
			{
				effected.sendPacket(SystemMessageId.RECOVERS_HP);
			}
			else if (effector.isPlayer() && (effector != effected))
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S2_HP_HAS_BEEN_RECOVERED_BY_C1);
				sm.Params.addString(effector.getName());
				sm.Params.addInt((int) amount);
				effected.sendPacket(sm);
			}
			else
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_VE_RECOVERED_S1_HP);
				sm.Params.addInt((int) amount);
				effected.sendPacket(sm);
			}
		}
	}
}