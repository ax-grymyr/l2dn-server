using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.InstanceZones.Conditions;

/**
 * Distance instance condition
 * @author malyelfik
 */
public class ConditionDistance: Condition
{
	public ConditionDistance(InstanceTemplate template, StatSet parameters, bool onlyLeader, bool showMessageAndHtml):base(template, parameters, onlyLeader, showMessageAndHtml)
	{
		setSystemMessage(SystemMessageId.C1_IS_TOO_FAR_FROM_THE_INSTANCE_ZONE_ENTRANCE, (message, player) => message.Params.addString(player.getName()));
	}
	
	protected override bool test(Player player, Npc npc)
	{
		int distance = getParameters().getInt("distance", 1000);
		return player.isInsideRadius3D(npc.getLocation().Location3D, distance);
	}
}