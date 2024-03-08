using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * StatByMoveType effect implementation.
 * @author UnAfraid
 */
public class StatByMoveType: AbstractEffect
{
	private readonly Stat _stat;
	private readonly MoveType _type;
	private readonly double _value;
	
	public StatByMoveType(StatSet @params)
	{
		_stat = @params.getEnum<Stat>("stat");
		_type = @params.getEnum<MoveType>("type");
		_value = @params.getDouble("value");
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.getStat().mergeMoveTypeValue(_stat, _type, _value);
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.getStat().mergeMoveTypeValue(_stat, _type, -_value);
	}
	
	public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item item)
	{
		return skill.isPassive() || skill.isToggle();
	}
}