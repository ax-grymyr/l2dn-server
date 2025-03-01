using System.Xml.Linq;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Items;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Quests.NewQuestData;

/**
 * @author Magik
 */
public class NewQuest
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(NewQuest));

	private readonly int _id;
	private readonly int _questType;
	private readonly string _name;
	private readonly int _startNpcId;
	private readonly int _endNpcId;
	private readonly int _startItemId;
	private readonly NewQuestLocation _location;
	private readonly NewQuestCondition _conditions;
	private readonly NewQuestGoal _goal;
	private readonly NewQuestReward _rewards;

	public NewQuest(XElement element)
	{
		_id = element.Attribute("id").GetInt32(-1);
		_questType = element.Attribute("type").GetInt32(-1);
		_name = element.Attribute("name").GetString("");
		_startNpcId = element.Attribute("startNpcId").GetInt32(-1);
		_endNpcId = element.Attribute("endNpcId").GetInt32(-1);
		_startItemId = element.Attribute("startItemId").GetInt32(-1);

		XElement? locationElement = element.Elements("locations").SingleOrDefault();
		_location = new NewQuestLocation((locationElement?.Attribute("startLocationId")).GetInt32(0),
			(locationElement?.Attribute("endLocationId")).GetInt32(0));

		XElement? conditionElement = element.Elements("conditions").SingleOrDefault();
		_conditions = new NewQuestCondition(conditionElement);

		XElement? goalsElement = element.Elements("goals").SingleOrDefault();
		_goal = new NewQuestGoal(goalsElement);

		if (_goal.getItemId() > 0)
		{
			ItemTemplate? template = ItemData.getInstance().getTemplate(_goal.getItemId());
			if (template == null)
			{
				LOGGER.Error(GetType().Name + _id + ": Could not find goal item template with id " + _goal.getItemId());
			}
			else
			{
				if (!template.isStackable())
				{
					LOGGER.Error(
						GetType().Name + _id + ": Item template with id " + _goal.getItemId() + " should be stackable.");
				}

				if (!template.isQuestItem())
				{
					LOGGER.Error(GetType().Name + _id + ": Item template with id " + _goal.getItemId() +
					            " should be quest item.");
				}
			}
		}

		XElement? rewardsElement = element.Elements("rewards").SingleOrDefault();
		_rewards = new NewQuestReward(rewardsElement);
	}

	public int getId()
	{
		return _id;
	}

	public int getQuestType()
	{
		return _questType;
	}

	public string getName()
	{
		return _name;
	}

	public int getStartNpcId()
	{
		return _startNpcId;
	}

	public int getEndNpcId()
	{
		return _endNpcId;
	}

	public int getStartItemId()
	{
		return _startItemId;
	}

	public NewQuestLocation getLocation()
	{
		return _location;
	}

	public NewQuestCondition getConditions()
	{
		return _conditions;
	}

	public NewQuestGoal getGoal()
	{
		return _goal;
	}

	public NewQuestReward getRewards()
	{
		return _rewards;
	}
}