using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Model;
using L2Dn.Model.DataPack;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class ActionData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(ActionData));

	private FrozenDictionary<int, ActionDataHolder> _actionData = FrozenDictionary<int, ActionDataHolder>.Empty;
	private FrozenDictionary<int, int> _actionSkillsData = FrozenDictionary<int, int>.Empty; // skillId, actionId

	private ActionData()
	{
		load();
	}

	public void load()
	{
		XmlActionData document = LoadXmlDocument<XmlActionData>(DataFileLocation.Data, "ActionData.xml");

		_actionData = document.Actions
			.Select(action => new ActionDataHolder(action.Id, action.Handler, action.Option))
			.ToFrozenDictionary(action => action.getId());

		Dictionary<int, int> actionSkillsData = new Dictionary<int, int>();
		foreach (XmlAction action in document.Actions)
		{
			if (action.Handler is "PetSkillUse" or "ServitorSkillUse" && action.Option > 0)
				actionSkillsData[action.Option] = action.Id;
		}

		_actionSkillsData = actionSkillsData.ToFrozenDictionary();

		_logger.Info(GetType().Name + ": Loaded " + _actionData.Count + " player actions.");
	}

	/**
	 * @param id
	 * @return the ActionDataHolder for specified id
	 */
	public ActionDataHolder? getActionData(int id)
	{
		return _actionData.GetValueOrDefault(id);
	}

	/**
	 * @param skillId
	 * @return the actionId corresponding to the skillId or -1 if no actionId is found for the specified skill.
	 */
	public int getSkillActionId(int skillId)
	{
		return _actionSkillsData.GetValueOrDefault(skillId, -1);
	}

	public ImmutableArray<int> getActionIdList()
	{
		return _actionData.Keys;
	}

	/**
	 * Gets the single instance of ActionData.
	 * @return single instance of ActionData
	 */
	public static ActionData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly ActionData INSTANCE = new();
	}
}