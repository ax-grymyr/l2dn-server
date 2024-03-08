using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * MagicalAttack-damage over time effect implementation.
 * @author Nik
 */
public class MagicalDamOverTime: AbstractEffect
{
	private readonly double _power;
	private readonly bool _canKill;
	
	public MagicalDamOverTime(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
		_canKill = @params.getBoolean("canKill", false);
		setTicks(@params.getInt("ticks"));
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.MAGICAL_DMG_OVER_TIME;
	}
	
	public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item item)
	{
		Creature creature = effector;
		Creature target = effected;
		
		if (target.isDead())
		{
			return false;
		}
		
		double damage = Formulas.calcMagicDam(creature, target, skill, creature.getMAtk(), _power, target.getMDef(), false, false, false); // In retail spiritshots change nothing.
		damage *= getTicksMultiplier();
		
		if (damage >= (target.getCurrentHp() - 1))
		{
			if (skill.isToggle())
			{
				target.sendPacket(SystemMessageId.YOUR_SKILL_HAS_BEEN_CANCELED_DUE_TO_LACK_OF_HP);
				return false;
			}
			
			// For DOT skills that will not kill effected player.
			if (!_canKill)
			{
				// Fix for players dying by DOTs if HP < 1 since reduceCurrentHP method will kill them
				if (target.getCurrentHp() <= 1)
				{
					return skill.isToggle();
				}
				damage = target.getCurrentHp() - 1;
			}
		}
		
		effector.doAttack(damage, effected, skill, true, false, false, false);
		return skill.isToggle();
	}
}