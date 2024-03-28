using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.Model;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Quests.NewQuestData;

/**
 * @author Magik
 */
public class NewQuestCondition
{
	private readonly int _minLevel;
	private readonly int _maxLevel;
	private readonly List<int> _previousQuestIds;
	private readonly List<CharacterClass> _allowedClassIds;
	private readonly bool _oneOfPreQuests;
	private readonly bool _specificStart;

	public NewQuestCondition(XElement? element)
	{
		List<CharacterClass> classRestriction = new();
		List<int> preQuestIds = new();
		int minLevel = -1;
		int maxLevel = ExperienceData.getInstance().getMaxLevel();
		bool oneOfPreQuests = false;
		bool specificStart = false;
		
		element?.Elements("param").ForEach(el =>
		{
			string name = el.GetAttributeValueAsString("name");
			string value = (string)el;

			switch (name)
			{
				case "classIds":
				{
					if (!string.IsNullOrEmpty(value))
						classRestriction.AddRange(value.Split(";").Select(x => (CharacterClass)int.Parse(x)));

					break;
				}
				case "preQuestId":
				{
					if (!string.IsNullOrEmpty(value))
						preQuestIds.AddRange(value.Split(";").Select(int.Parse));

					break;
				}
				case "minLevel":
				{
					minLevel = int.Parse(value);
					break;
				}
				case "maxLevel":
				{
					maxLevel = int.Parse(value);
					break;
				}
				case "oneOfPreQuests":
				{
					oneOfPreQuests = bool.Parse(value);
					break;
				}
				case "specificStart":
				{
					specificStart = bool.Parse(value);
					break;
				}
			}
		});

		_minLevel = minLevel;
		_maxLevel = maxLevel;
		_previousQuestIds = preQuestIds;
		_allowedClassIds = classRestriction;
		_oneOfPreQuests = oneOfPreQuests;
		_specificStart = specificStart;
	}
	
	public NewQuestCondition(int minLevel, int maxLevel, List<int> previousQuestIds, List<CharacterClass> allowedClassIds,
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

	public List<CharacterClass> getAllowedClassIds()
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