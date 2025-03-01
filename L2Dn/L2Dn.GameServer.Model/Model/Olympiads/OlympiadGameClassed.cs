using L2Dn.Extensions;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Olympiads;

/**
 * @author DS
 */
public class OlympiadGameClassed: OlympiadGameNormal
{
	private OlympiadGameClassed(int id, Participant[] opponents): base(id, opponents)
	{
	}

	public override CompetitionType getType()
	{
		return CompetitionType.CLASSED;
	}

	protected override int getDivider()
	{
		return Config.ALT_OLY_DIVIDER_CLASSED;
	}

	public static OlympiadGameClassed? createGame(int id, List<Set<int>> classList)
	{
		if (classList.Count == 0)
			return null;

        while (classList.Count != 0)
		{
			Set<int> list = classList.GetRandomElement();
			if (list.Count < 2)
			{
				classList.Remove(list);
				continue;
			}

			Participant[]? opponents = createListOfParticipants(list);
			if (opponents == null)
			{
				classList.Remove(list);
				continue;
			}

			return new OlympiadGameClassed(id, opponents);
		}

		return null;
	}
}