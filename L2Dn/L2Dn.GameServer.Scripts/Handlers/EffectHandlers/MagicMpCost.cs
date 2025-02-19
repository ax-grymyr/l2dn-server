using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class MagicMpCost: AbstractEffect
{
	private readonly int _magicType;
	private readonly double _amount;

	public MagicMpCost(StatSet @params)
	{
		_magicType = @params.getInt("magicType", 0);
		_amount = @params.getDouble("amount", 0);
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
	{
		effected.getStat().mergeMpConsumeTypeValue(_magicType, _amount / 100 + 1, (a, b) => a * b);
	}

	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.getStat().mergeMpConsumeTypeValue(_magicType, _amount / 100 + 1, (a, b) => a / b);
	}
}