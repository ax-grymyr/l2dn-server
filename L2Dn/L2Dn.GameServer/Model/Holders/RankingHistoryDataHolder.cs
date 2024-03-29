namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Serenitty
 */
public class RankingHistoryDataHolder
{
	private readonly int _rank;
	private readonly long _exp;
	private readonly DateOnly _day;

	public RankingHistoryDataHolder(DateOnly day, int rank, long exp)
	{
		_day = day;
		_rank = rank;
		_exp = exp;
	}

	public DateOnly getDay()
	{
		return _day;
	}

	public int getRank()
	{
		return _rank;
	}

	public long getExp()
	{
		return _exp;
	}
}