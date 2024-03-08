using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * An effect that sets the current hp to the given amount.
 * @author Nik
 */
public class SetHp: AbstractEffect
{
	private readonly double _amount;
	private readonly StatModifierType _mode;
	
	public SetHp(StatSet @params)
	{
		_amount = @params.getDouble("amount", 0);
		_mode = @params.getEnum("mode", StatModifierType.DIFF);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isDead() || effected.isDoor())
		{
			return;
		}
		
		bool full = (_mode == StatModifierType.PER) && (_amount == 100.0);
		double amount = full ? effected.getMaxHp() : (_mode == StatModifierType.PER) ? ((effected.getMaxHp() * _amount) / 100.0) : _amount;
		effected.setCurrentHp(amount);
	}
}