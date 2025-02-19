using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Mana Heal Over Time effect implementation.
 */
public class ManaHealOverTime: AbstractEffect
{
	private readonly double _power;

	public ManaHealOverTime(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
		setTicks(@params.getInt("ticks"));
	}

	public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item? item)
	{
		if (effected.isDead())
		{
			return false;
		}

		double mp = effected.getCurrentMp();
		double maxmp = effected.getMaxRecoverableMp();

		// Not needed to set the MP and send update packet if player is already at max MP
		if (_power > 0)
		{
			if (mp >= maxmp)
			{
				return true;
			}
		}
		else
		{
			if (mp - _power <= 0)
			{
				return true;
			}
		}

		double power = _power;
		if (item != null && (item.isPotion() || item.isElixir()))
		{
			power += effected.getStat().getValue(Stat.ADDITIONAL_POTION_MP, 0) / getTicks();
		}

		mp += power * getTicksMultiplier();
		if (_power > 0)
		{
			mp = Math.Min(mp, maxmp);
		}
		else
		{
			mp = Math.Max(mp, 1);
		}
		effected.setCurrentMp(mp, false);
		effected.broadcastStatusUpdate(effector);
		return skill.isToggle();
	}
}