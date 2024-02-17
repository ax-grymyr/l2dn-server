using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.InstanceZones.Conditions;

/**
 * Instance enter group max size
 * @author malyelfik
 */
public class ConditionGroupMax: Condition
{
	public ConditionGroupMax(InstanceTemplate template, StatSet parameters, bool onlyLeader, bool showMessageAndHtml):base(template, parameters, true, showMessageAndHtml)
	{
		setSystemMessage(SystemMessageId.YOU_CANNOT_ENTER_DUE_TO_THE_PARTY_HAVING_EXCEEDED_THE_LIMIT);
	}
	
	protected override bool test(Player player, Npc npc, List<Player> group)
	{
		return group.Count <= getLimit();
	}
	
	public int getLimit()
	{
		return getParameters().getInt("limit");
	}
}