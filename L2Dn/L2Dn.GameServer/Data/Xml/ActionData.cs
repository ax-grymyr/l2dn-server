using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class ActionData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ActionData));
	
	private readonly Map<int, ActionDataHolder> _actionData = new();
	private readonly Map<int, int> _actionSkillsData = new(); // skillId, actionId
	
	protected ActionData()
	{
		load();
	}
	
	public void load()
	{
		_actionData.clear();
		_actionSkillsData.clear();
		parseDatapackFile("data/ActionData.xml");
		
		foreach (ActionDataHolder holder in _actionData.values())
		{
			if (holder.getHandler().equals("PetSkillUse") || holder.getHandler().equals("ServitorSkillUse"))
			{
				_actionSkillsData.put(holder.getOptionId(), holder.getId());
			}
		}
		
		LOGGER.Info(GetType().Name + ": Loaded " + _actionData.size() + " player actions.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "action", actionNode =>
		{
			ActionDataHolder holder = new ActionDataHolder(new StatSet(parseAttributes(actionNode)));
			_actionData.put(holder.getId(), holder);
		}));
	}
	
	/**
	 * @param id
	 * @return the ActionDataHolder for specified id
	 */
	public ActionDataHolder getActionData(int id)
	{
		return _actionData.get(id);
	}
	
	/**
	 * @param skillId
	 * @return the actionId corresponding to the skillId or -1 if no actionId is found for the specified skill.
	 */
	public int getSkillActionId(int skillId)
	{
		return _actionSkillsData.getOrDefault(skillId, -1);
	}
	
	public int[] getActionIdList()
	{
		return _actionData.Keys.ToArray();
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
		public static readonly ActionData INSTANCE = new ActionData();
	}
}