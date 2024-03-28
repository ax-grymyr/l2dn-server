using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.InstanceZones.Conditions;

/**
 * Instance no party condition
 * @author St3eT
 */
public class ConditionNoParty: Condition
{
	public ConditionNoParty(InstanceTemplate template, StatSet parameters, bool onlyLeader, bool showMessageAndHtml):
		base(template, parameters, true, showMessageAndHtml)
	{
	}

	protected override bool test(Player player, Npc npc)
	{
		return !player.isInParty();
	}
}