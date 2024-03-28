namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Serenitty, Macuk
 */
public class ChallengePointInfoHolder
{
	private readonly int _pointGroupId;
	private int _challengePoint;
	private readonly int _ticketPointOpt1;
	private readonly int _ticketPointOpt2;
	private readonly int _ticketPointOpt3;
	private readonly int _ticketPointOpt4;
	private readonly int _ticketPointOpt5;
	private readonly int _ticketPointOpt6;

	public ChallengePointInfoHolder(int pointGroupId, int challengePoint, int ticketPointOpt1, int ticketPointOpt2,
		int ticketPointOpt3, int ticketPointOpt4, int ticketPointOpt5, int ticketPointOpt6)
	{
		_pointGroupId = pointGroupId;
		_challengePoint = challengePoint;
		_ticketPointOpt1 = ticketPointOpt1;
		_ticketPointOpt2 = ticketPointOpt2;
		_ticketPointOpt3 = ticketPointOpt3;
		_ticketPointOpt4 = ticketPointOpt4;
		_ticketPointOpt5 = ticketPointOpt5;
		_ticketPointOpt6 = ticketPointOpt6;
	}

	public int getPointGroupId()
	{
		return _pointGroupId;
	}

	public int getChallengePoint()
	{
		return _challengePoint;
	}

	public int getTicketPointOpt1()
	{
		return _ticketPointOpt1;
	}

	public int getTicketPointOpt2()
	{
		return _ticketPointOpt2;
	}

	public int getTicketPointOpt3()
	{
		return _ticketPointOpt3;
	}

	public int getTicketPointOpt4()
	{
		return _ticketPointOpt4;
	}

	public int getTicketPointOpt5()
	{
		return _ticketPointOpt5;
	}

	public int getTicketPointOpt6()
	{
		return _ticketPointOpt6;
	}

	public void addPoints(int points)
	{
		_challengePoint += points;
	}
}