using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * An effect that changes damage taken from an attack.<br>
 * The retail implementation seems to be altering whatever damage is taken after the attack has been done and not when attack is being done.<br>
 * Exceptions for this effect appears to be DOT effects and terrain damage, they are unaffected by this stat.<br>
 * As for example in retail this effect does reduce reflected damage taken (because it is received damage), as well as it does not decrease reflected damage done,<br>
 * because reflected damage is being calculated with the original attack damage and not this altered one.<br>
 * Multiple values of this effect add-up to each other rather than multiplying with each other. Be careful, there were cases in retail where damage is deacreased to 0.
 * @author Nik
 */
public class DamageByAttack: AbstractEffect
{
	private readonly double _value;
	private readonly DamageByAttackType _type;
	
	public DamageByAttack(StatSet @params)
	{
		_value = @params.getDouble("amount");
		_type = @params.getEnum("type", DamageByAttackType.NONE);
		if (@params.getEnum("mode", StatModifierType.DIFF) != StatModifierType.DIFF)
		{
			LOGGER.Warn(GetType().Name + " can only use DIFF mode.");
		}
	}
	
	public override void pump(Creature target, Skill skill)
	{
		switch (_type)
		{
			case DamageByAttackType.PK:
			{
				target.getStat().mergeAdd(Stat.PVP_DAMAGE_TAKEN, _value);
				break;
			}
			case DamageByAttackType.ENEMY_ALL:
			{
				target.getStat().mergeAdd(Stat.PVE_DAMAGE_TAKEN, _value);
				break;
			}
			case DamageByAttackType.MOB:
			{
				target.getStat().mergeAdd(Stat.PVE_DAMAGE_TAKEN_MONSTER, _value);
				break;
			}
			case DamageByAttackType.BOSS:
			{
				target.getStat().mergeAdd(Stat.PVE_DAMAGE_TAKEN_RAID, _value);
				break;
			}
		}
	}
}