using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;

namespace L2Dn.GameServer.Model.InstanceZones.Conditions;

/**
 * Instance residence condition
 * @author malyelfik
 */
public class ConditionHasResidence: Condition
{
	public ConditionHasResidence(InstanceTemplate template, StatSet parameters, bool onlyLeader,
		bool showMessageAndHtml): base(template, parameters, onlyLeader, showMessageAndHtml)
	{
	}

	protected override bool test(Player player, Npc npc)
	{
		Clan? clan = player.getClan();
		if (clan == null)
		{
			return false;
		}

		bool test = false;
		StatSet @params = getParameters();
		int id = @params.getInt("id");
		ResidenceType type = @params.getEnum<ResidenceType>("type");
		switch (type)
		{
			case ResidenceType.CASTLE:
			{
				test = clan.getCastleId() == id;
				break;
			}
			case ResidenceType.FORTRESS:
			{
				test = clan.getFortId() == id;
				break;
			}
			case ResidenceType.CLANHALL:
			{
				test = clan.getHideoutId() == id;
				break;
			}
		}

		return test;
	}
}