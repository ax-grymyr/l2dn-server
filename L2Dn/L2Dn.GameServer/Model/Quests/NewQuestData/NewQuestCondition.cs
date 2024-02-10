using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Quests.NewQuestData;

/**
 * @author Magik
 */
public class NewQuestCondition
{
	private readonly int _minLevel;
	private readonly int _maxLevel;
	private readonly List<int> _previousQuestIds;
	private readonly List<ClassId> _allowedClassIds;
	private readonly bool _oneOfPreQuests;
	private readonly bool _specificStart;

	public NewQuestCondition(int minLevel, int maxLevel, List<int> previousQuestIds, List<ClassId> allowedClassIds,
		bool oneOfPreQuests, bool specificStart)
	{
		_minLevel = minLevel;
		_maxLevel = maxLevel;
		_previousQuestIds = previousQuestIds;
		_allowedClassIds = allowedClassIds;
		_oneOfPreQuests = oneOfPreQuests;
		_specificStart = specificStart;
	}

	public int getMinLevel()
	{
		return _minLevel;
	}

	public int getMaxLevel()
	{
		return _maxLevel;
	}

	public List<int> getPreviousQuestIds()
	{
		return _previousQuestIds;
	}

	public List<ClassId> getAllowedClassIds()
	{
		return _allowedClassIds;
	}

	public bool getOneOfPreQuests()
	{
		return _oneOfPreQuests;
	}

	public bool getSpecificStart()
	{
		return _specificStart;
	}
}