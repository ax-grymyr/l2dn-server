using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw, Mobius
 */
public class RealDamage: AbstractEffect
{
	private readonly double _power;
	private readonly StatModifierType _mode;
	
	public RealDamage(StatSet @params)
	{
		if (@params.getDouble("amount", 0) > 0)
		{
			LOGGER.Warn(GetType().Name + " should use power instead of amount.");
		}
		_power = @params.getDouble("power", 0);
		_mode = @params.getEnum("mode", StatModifierType.DIFF);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isDead() || effected.isDoor() || effected.isRaid())
		{
			return;
		}
		
		// Check if effected NPC is not attackable.
		if (effected.isNpc() && !effected.isAttackable())
		{
			return;
		}
		
		// Check if fake players should aggro each other.
		if (effector.isFakePlayer() && !Config.FAKE_PLAYER_AGGRO_FPC && effected.isFakePlayer())
		{
			return;
		}
		
		// Calculate resistance.
		double damage;
		if (_mode == StatModifierType.DIFF)
		{
			damage = _power - (_power * (Math.Min(effected.getStat().getMulValue(Stat.REAL_DAMAGE_RESIST, 1), 1.8) - 1));
		}
		else // PER
		{
			// Percent does not ignore HP block.
			if (effected.isHpBlocked())
			{
				return;
			}
			
			// Percent Level check https://eu.4game.com/patchnotes/lineage2/270/ Changed Skills
			int levelDifference = Math.Abs(effector.getLevel() - effected.getLevel());
			if (levelDifference >= 6)
			{
				return;
			}
			
			if (levelDifference >= 3)
			{
				damage = ((effected.getCurrentHp() * _power) / 100) / levelDifference;
			}
			else
			{
				damage = (effected.getCurrentHp() * _power) / 100;
			}
		}
		
		// Do damage.
		if (damage > 0)
		{
			effected.setCurrentHp(Math.Max(effected.getCurrentHp() - damage, effected.isUndying() ? 1 : 0));
			
			// Die.
			if ((effected.getCurrentHp() < 0.5) && (!effected.isPlayer() || !effected.getActingPlayer().isInOlympiadMode()))
			{
				effected.doDie(effector);
			}
		}
		
		// Send message.
		if (effector.isPlayer())
		{
			effector.sendDamageMessage(effected, skill, (int) damage, 0, false, false, false);
		}
	}
}