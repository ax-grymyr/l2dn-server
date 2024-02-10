namespace L2Dn.GameServer.Model.Quests.NewQuestData;

/**
 * @author Magik
 */
public class NewQuestLocation
{
	private readonly int _startLocationId;
	private readonly int _endLocationId;

	public NewQuestLocation(int startLocationId, int endLocationId)
	{
		_startLocationId = startLocationId;
		_endLocationId = endLocationId;
	}

	public int getStartLocationId()
	{
		return _startLocationId;
	}

	public int getEndLocationId()
	{
		return _endLocationId;
	}
}