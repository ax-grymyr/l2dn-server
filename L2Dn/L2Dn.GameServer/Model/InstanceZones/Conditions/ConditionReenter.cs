using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.InstanceZones.Conditions;

/**
 * Instance reenter conditions
 * @author malyelfik
 */
public class ConditionReenter: Condition
{
	public ConditionReenter(InstanceTemplate template, StatSet parameters, bool onlyLeader, bool showMessageAndHtml):
		base(template, parameters, onlyLeader, showMessageAndHtml)
	{
		setSystemMessage(SystemMessageId.C1_CANNOT_ENTER_YET, (message, player) => message.Params.addString(player.getName()));
	}

	protected override bool test(Player player, Npc npc)
	{
		int instanceId = getParameters().getInt("instanceId", getInstanceTemplate().getId());
		return DateTime.UtcNow > InstanceManager.getInstance().getInstanceTime(player, instanceId);
	}
}