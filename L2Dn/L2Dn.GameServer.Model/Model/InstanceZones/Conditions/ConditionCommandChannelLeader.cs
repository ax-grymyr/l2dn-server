using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.InstanceZones.Conditions;

/**
 * Command channel leader condition
 * @author malyelfik
 */
public class ConditionCommandChannelLeader: Condition
{
	public ConditionCommandChannelLeader(InstanceTemplate template, StatSet parameters, bool onlyLeader,
		bool showMessageAndHtml)
		: base(template, parameters, true, showMessageAndHtml)
	{
		setSystemMessage(SystemMessageId.ONLY_A_PARTY_LEADER_CAN_MAKE_THE_REQUEST_TO_ENTER);
	}

	protected override bool test(Player player, Npc npc)
	{
		AbstractPlayerGroup group = player.getCommandChannel();
		return group != null && group.isLeader(player);
	}
}