using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class SoulSavedSkillCondition: ISkillCondition
{
	private readonly SoulType _type;
	private readonly int _amount;
	
	public SoulSavedSkillCondition(StatSet @params)
	{
		_type = @params.getEnum("type", SoulType.LIGHT);
		_amount = @params.getInt("amount");
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		return caster.isPlayer() && (caster.getActingPlayer().getChargedSouls(_type) >= _amount);
	}
}