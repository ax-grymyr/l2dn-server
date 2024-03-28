using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.InstanceZones.Conditions;

/**
 * Command channel condition
 * @author malyelfik
 */
public class ConditionCommandChannel: Condition
{
	public ConditionCommandChannel(InstanceTemplate template, StatSet parameters, bool onlyLeader,
		bool showMessageAndHtml): base(template, parameters, true, showMessageAndHtml)
	{
		setSystemMessage(SystemMessageId
			.YOU_CANNOT_ENTER_BECAUSE_YOU_ARE_NOT_ASSOCIATED_WITH_THE_CURRENT_COMMAND_CHANNEL);
	}

	protected override bool test(Player player, Npc npc)
	{
		return player.isInCommandChannel();
	}
}