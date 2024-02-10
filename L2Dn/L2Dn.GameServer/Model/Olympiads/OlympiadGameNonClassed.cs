using L2Dn.GameServer.Utilities;

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
		return Config.ALT_OLY_DIVIDER_NON_CLASSED;
	}

	protected static OlympiadGameNonClassed createGame(int id, Set<int> list)
	{
		Participant[] opponents = createListOfParticipants(list);
		if (opponents == null)
		{
			return null;
		}

		return new OlympiadGameNonClassed(id, opponents);
	}
}