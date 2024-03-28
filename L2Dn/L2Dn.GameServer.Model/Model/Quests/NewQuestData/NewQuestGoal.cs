using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Quests.NewQuestData;

/**
 * @author Magik
 */
public class NewQuestGoal
{
	private readonly int _itemId;
	private readonly int _count;
	private readonly String _goalMessage;

	public NewQuestGoal(XElement? element)
	{
		int goalItemId = -1;
		int goalCount = -1;
		string goalString = string.Empty;

		element?.Elements("param").ForEach(el =>
		{
			string name = el.GetAttributeValueAsString("name");
			switch (name)
			{
				case "goalItemId":
				{
					goalItemId = (int)el;
					break;
				}
				case "goalCount":
				{
					goalCount = (int)el;
					break;
				}
				case "goalString":
				{
					goalString = (string)el;
					break;
				}
			}
		});

		_itemId = goalItemId;
		_count = goalCount;
		_goalMessage = goalString;
	}

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