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
	
	protected static OlympiadGameClassed createGame(int id, List<Set<int>> classList)
	{
		if ((classList == null) || classList.isEmpty())
		{
			return null;
		}
		
		Set<int> list;
		Participant[] opponents;
		while (!classList.isEmpty())
		{
			list = classList.get(Rnd.get(classList.size()));
			if ((list == null) || (list.size() < 2))
			{
				classList.Remove(list);
				continue;
			}
			
			opponents = OlympiadGameNormal.createListOfParticipants(list);
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