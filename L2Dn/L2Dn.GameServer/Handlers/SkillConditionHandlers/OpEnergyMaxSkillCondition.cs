using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class OpEnergyMaxSkillCondition: ISkillCondition
{
	private readonly int _amount;
	
	public OpEnergyMaxSkillCondition(StatSet @params)
	{
		_amount = @params.getInt("amount");
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		if (caster.getActingPlayer().getCharges() >= _amount)
		{
			caster.sendPacket(SystemMessageId.YOUR_FORCE_HAS_REACHED_MAXIMUM_CAPACITY);
			return false;
		}
		return true;
	}
}