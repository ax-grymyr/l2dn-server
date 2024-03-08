using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class ReflectSkill: AbstractEffect
{
	private readonly Stat _stat;
	private readonly double _amount;
	
	public ReflectSkill(StatSet @params)
	{
		_stat = @params.getEnum("type", BasicProperty.PHYSICAL) == BasicProperty.PHYSICAL ? Stat.REFLECT_SKILL_PHYSIC : Stat.REFLECT_SKILL_MAGIC;
		_amount = @params.getDouble("amount", 0);
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		effected.getStat().mergeAdd(_stat, _amount);
	}
}