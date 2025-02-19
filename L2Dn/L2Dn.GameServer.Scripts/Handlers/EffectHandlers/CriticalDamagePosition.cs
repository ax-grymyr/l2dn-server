using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Geometry;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class CriticalDamagePosition: AbstractEffect
{
	private readonly double _amount;
	private readonly Position _position;

	public CriticalDamagePosition(StatSet @params)
	{
		_amount = @params.getDouble("amount", 0);
		_position = @params.getEnum("position", Position.Front);
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
	{
		effected.getStat().mergePositionTypeValue(Stat.CRITICAL_DAMAGE, _position, _amount / 100 + 1, (a, b) => a * b);
	}

	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.getStat().mergePositionTypeValue(Stat.CRITICAL_DAMAGE, _position, _amount / 100 + 1, (a, b) => a / b);
	}
}