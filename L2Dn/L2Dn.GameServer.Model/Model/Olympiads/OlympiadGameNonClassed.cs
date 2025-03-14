using L2Dn.GameServer.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Olympiads;

/**
 * @author DS
 */
public class OlympiadGameNonClassed: OlympiadGameNormal
{
	public OlympiadGameNonClassed(int id, Participant[] opponents): base(id, opponents)
	{
	}

	public override CompetitionType getType()
	{
		return CompetitionType.NON_CLASSED;
	}

	protected override int getDivider()
	{
		return Config.Olympiad.ALT_OLY_DIVIDER_NON_CLASSED;
	}

	public static OlympiadGameNonClassed? createGame(int id, Set<int> list)
	{
		Participant[]? opponents = createListOfParticipants(list);
		if (opponents == null)
		{
			return null;
		}

		return new OlympiadGameNonClassed(id, opponents);
	}
}