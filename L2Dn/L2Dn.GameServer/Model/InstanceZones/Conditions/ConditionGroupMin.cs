using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.InstanceZones.Conditions;

/**
 * Instance enter group min size
 * @author malyelfik
 */
public class ConditionGroupMin: Condition
{
	public ConditionGroupMin(InstanceTemplate template, StatSet parameters, bool onlyLeader, bool showMessageAndHtml):
		base(template, parameters, true, showMessageAndHtml)
	{
		setSystemMessage(SystemMessageId.YOU_MUST_HAVE_A_MINIMUM_OF_S1_PEOPLE_TO_ENTER_THIS_INSTANCE_ZONE,
			(msg, player) => msg.addInt(getLimit()));
	}

	protected override bool test(Player player, Npc npc, List<Player> group)
	{
		return group.Count >= getLimit();
	}

	public int getLimit()
	{
		return getParameters().getInt("limit");
	}
}