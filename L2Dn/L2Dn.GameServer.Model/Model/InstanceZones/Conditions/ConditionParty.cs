using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.InstanceZones.Conditions;

/**
 * Instance party condition
 * @author malyelfik
 */
public class ConditionParty: Condition
{
	public ConditionParty(InstanceTemplate template, StatSet parameters, bool onlyLeader, bool showMessageAndHtml):
		base(template, parameters, true, showMessageAndHtml)
	{
		setSystemMessage(SystemMessageId.YOU_ARE_NOT_IN_A_PARTY_SO_YOU_CANNOT_ENTER);
	}

	protected override bool test(Player player, Npc npc)
	{
		return player.isInParty();
	}
}