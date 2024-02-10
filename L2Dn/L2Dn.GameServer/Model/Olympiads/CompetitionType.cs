namespace L2Dn.GameServer.Model.Olympiads;

/**
 * @author DS
 */
public enum CompetitionType
{
	CLASSED,
	NON_CLASSED,
	OTHER
}

public static class CompetitionTypeUtil
{
	public static string toString(this CompetitionType competitionType)
	{
		return competitionType switch
		{
			CompetitionType.CLASSED => "classed",
			CompetitionType.NON_CLASSED => "non-classed",
			CompetitionType.OTHER => "other",
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}