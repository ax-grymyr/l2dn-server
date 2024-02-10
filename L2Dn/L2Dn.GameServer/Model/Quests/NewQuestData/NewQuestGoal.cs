namespace L2Dn.GameServer.Model.Quests.NewQuestData;

/**
 * @author Magik
 */
public class NewQuestGoal
{
	private readonly int _itemId;
	private readonly int _count;
	private readonly String _goalMessage;

	public NewQuestGoal(int itemId, int count, String goalMessage)
	{
		_itemId = itemId;
		_count = count;
		_goalMessage = goalMessage;
	}

	public int getItemId()
	{
		return _itemId;
	}

	public int getCount()
	{
		return _count;
	}

	public String getMessage()
	{
		return _goalMessage;
	}
}