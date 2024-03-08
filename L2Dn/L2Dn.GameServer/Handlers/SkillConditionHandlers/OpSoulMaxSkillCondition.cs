using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class OpSoulMaxSkillCondition: ISkillCondition
{
	private readonly SoulType _type;
	
	public OpSoulMaxSkillCondition(StatSet @params)
	{
		_type = @params.getEnum("type", SoulType.LIGHT);
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		int maxSouls = (int) caster.getStat().getValue(Stat.MAX_SOULS);
		return caster.isPlayable() && (caster.getActingPlayer().getChargedSouls(_type) < maxSouls);
	}
}