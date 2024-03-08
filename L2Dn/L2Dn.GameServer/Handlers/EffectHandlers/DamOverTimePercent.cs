using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Damage Over Time Percent effect implementation.
 * @author Adry_85
 */
public class DamOverTimePercent: AbstractEffect
{
	private readonly bool _canKill;
	private readonly double _power;
	
	public DamOverTimePercent(StatSet @params)
	{
		_canKill = @params.getBoolean("canKill", false);
		_power = @params.getDouble("power");
		setTicks(@params.getInt("ticks"));
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.DMG_OVER_TIME_PERCENT;
	}
	
	public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isDead())
		{
			return false;
		}
		
		double damage = effected.getCurrentHp() * _power * getTicksMultiplier();
		if (damage >= (effected.getCurrentHp() - 1))
		{
			if (skill.isToggle())
			{
				effected.sendPacket(SystemMessageId.YOUR_SKILL_HAS_BEEN_CANCELED_DUE_TO_LACK_OF_HP);
				return false;
			}
			
			// For DOT skills that will not kill effected player.
			if (!_canKill)
			{
				// Fix for players dying by DOTs if HP < 1 since reduceCurrentHP method will kill them
				if (effected.getCurrentHp() <= 1)
				{
					return skill.isToggle();
				}
				damage = effected.getCurrentHp() - 1;
			}
		}
		
		effector.doAttack(damage, effected, skill, true, false, false, false);
		return skill.isToggle();
	}
}