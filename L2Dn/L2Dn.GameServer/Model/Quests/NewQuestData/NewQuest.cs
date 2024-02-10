using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Utilities;
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
	private readonly String _name;
	private readonly int _startNpcId;
	private readonly int _endNpcId;
	private readonly int _startItemId;
	private readonly NewQuestLocation _location;
	private readonly NewQuestCondition _conditions;
	private readonly NewQuestGoal _goal;
	private readonly NewQuestReward _rewards;

	public NewQuest(StatSet set)
	{
		_id = set.getInt("id", -1);
		_questType = set.getInt("type", -1);
		_name = set.getString("name", "");
		_startNpcId = set.getInt("startNpcId", -1);
		_endNpcId = set.getInt("endNpcId", -1);
		_startItemId = set.getInt("startItemId", -1);
		_location = new NewQuestLocation(set.getInt("startLocationId", 0), set.getInt("endLocationId", 0));

		String classIds = set.getString("classIds", "");
		List<ClassId> classRestriction = classIds.isEmpty()
			? new()
			: classIds.Split(";").Select(it => (ClassId)(int.Parse(it))).ToList();
		String preQuestId = set.getString("preQuestId", "");
		List<int> preQuestIds =
			preQuestId.isEmpty() ? new() : preQuestId.Split(";").Select(it => int.Parse(it)).ToList();
		_conditions = new NewQuestCondition(set.getInt("minLevel", -1),
			set.getInt("maxLevel", ExperienceData.getInstance().getMaxLevel()), preQuestIds, classRestriction,
			set.getBoolean("oneOfPreQuests", false), set.getBoolean("specificStart", false));

		int goalItemId = set.getInt("goalItemId", -1);
		int goalCount = set.getInt("goalCount", -1);
		if (goalItemId > 0)
		{
			ItemTemplate template = ItemData.getInstance().getTemplate(goalItemId);
			if (template == null)
			{
				LOGGER.Warn(GetType().Name + _id + ": Could not find goal item template with id " + goalItemId);
			}
			else
			{
				if (!template.isStackable())
				{
					LOGGER.Warn(
						GetType().Name + _id + ": Item template with id " + goalItemId + " should be stackable.");
				}

				if (!template.isQuestItem())
				{
					LOGGER.Warn(GetType().Name + _id + ": Item template with id " + goalItemId +
					            " should be quest item.");
				}
			}
		}

		_goal = new NewQuestGoal(goalItemId, goalCount, set.getString("goalString", ""));

		_rewards = new NewQuestReward(set.getLong("rewardExp", -1), set.getLong("rewardSp", -1),
			set.getInt("rewardLevel", -1), set.getList<ItemHolder>("rewardItems"));
	}

	public int getId()
	{
		return _id;
	}

	public int getQuestType()
	{
		return _questType;
	}

	public String getName()
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