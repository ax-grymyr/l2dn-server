using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Sdw
 */
public class ConditionPlayerHasFreeSummonPoints : Condition
{
	private readonly int _summonPoints;
	
	public ConditionPlayerHasFreeSummonPoints(int summonPoints)
	{
		_summonPoints = summonPoints;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		Player player = effector.getActingPlayer();
		if (player == null)
		{
			return false;
		}
		
		bool canSummon = true;
		if ((_summonPoints == 0) && player.hasServitors())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_USE_THE_S1_SKILL_DUE_TO_INSUFFICIENT_SUMMON_POINTS);
			canSummon = false;
		}
		else if ((player.getSummonPoints() + _summonPoints) > player.getMaxSummonPoints())
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_CANNOT_USE_THE_S1_SKILL_DUE_TO_INSUFFICIENT_SUMMON_POINTS);
			sm.Params.addSkillName(skill);
			player.sendPacket(sm);
			canSummon = false;
		}
		
		return canSummon;
	}
}
