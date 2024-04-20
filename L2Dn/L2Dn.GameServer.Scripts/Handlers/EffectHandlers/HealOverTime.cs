using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Heal Over Time effect implementation.
 */
public class HealOverTime: AbstractEffect
{
	private readonly double _power;
	
	public HealOverTime(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
		setTicks(@params.getInt("ticks"));
	}
	
	public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isDead() || effected.isDoor())
		{
			return false;
		}
		
		double hp = effected.getCurrentHp();
		double maxhp = effected.getMaxRecoverableHp();
		
		// Not needed to set the HP and send update packet if player is already at max HP
		if (_power > 0)
		{
			if (hp >= maxhp)
			{
				return false;
			}
		}
		else
		{
			if ((hp - _power) <= 0)
			{
				return false;
			}
		}
		
		double power = _power;
		if ((item != null) && (item.isPotion() || item.isElixir()))
		{
			power += effected.getStat().getValue(Stat.ADDITIONAL_POTION_HP, 0) / getTicks();
			
			// Classic Potion Mastery
			// TODO: Create an effect if more mastery skills are added.
			power *= 1.0 + (effected.getAffectedSkillLevel((int)CommonSkill.POTION_MASTERY) / 100.0);
		}
		
		hp += power * getTicksMultiplier();
		if (_power > 0)
		{
			hp = Math.Min(hp, maxhp);
		}
		else
		{
			hp = Math.Max(hp, 1);
		}
		effected.setCurrentHp(hp, false);
		effected.broadcastStatusUpdate(effector);
		return skill.isToggle();
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isPlayer() && (getTicks() > 0) && (skill.getAbnormalType() == AbnormalType.HP_RECOVER))
		{
			double power = _power;
			if ((item != null) && (item.isPotion() || item.isElixir()))
			{
				double bonus = effected.getStat().getValue(Stat.ADDITIONAL_POTION_HP, 0);
				if (bonus > 0)
				{
					power += bonus / getTicks();
				}
			}

			effected.sendPacket(new ExRegenMaxPacket((int)(skill.getAbnormalTime() ?? TimeSpan.Zero).TotalSeconds,
				getTicks(), power));
		}
	}
}