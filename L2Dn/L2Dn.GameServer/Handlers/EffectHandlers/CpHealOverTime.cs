using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Cp Heal Over Time effect implementation.
 */
public class CpHealOverTime: AbstractEffect
{
	private readonly double _power;
	
	public CpHealOverTime(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
		setTicks(@params.getInt("ticks"));
	}
	
	public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isDead())
		{
			return false;
		}
		
		double cp = effected.getCurrentCp();
		double maxcp = effected.getMaxRecoverableCp();
		
		// Not needed to set the CP and send update packet if player is already at max CP
		if (_power > 0)
		{
			if (cp >= maxcp)
			{
				return false;
			}
		}
		else
		{
			if ((cp - _power) <= 0)
			{
				return false;
			}
		}
		
		double power = _power;
		if ((item != null) && (item.isPotion() || item.isElixir()))
		{
			power += effected.getStat().getValue(Stat.ADDITIONAL_POTION_CP, 0) / getTicks();
		}
		
		cp += power * getTicksMultiplier();
		if (_power > 0)
		{
			cp = Math.Min(cp, maxcp);
		}
		else
		{
			cp = Math.Max(cp, 1);
		}
		effected.setCurrentCp(cp, false);
		effected.broadcastStatusUpdate(effector);
		return true;
	}
}