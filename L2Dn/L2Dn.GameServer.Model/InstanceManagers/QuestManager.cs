using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Quests and scripts manager.
 * @author Zoey76
 */
public class QuestManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(QuestManager));
	
	/** Map containing all the quests. */
	private readonly Map<string, Quest> _quests = new();
	
	protected QuestManager()
	{
	}
	
	public bool reload(string questName)
	{
		Quest q = getQuest(questName);
		if (q == null)
		{
			return false;
		}
		
		q.Reload();
		return true;
	}
	
	/**
	 * Reloads a the quest by ID.
	 * @param questId the ID of the quest to be reloaded
	 * @return {@code true} if reload was successful, {@code false} otherwise
	 */
	public bool reload(int questId)
	{
		Quest q = getQuest(questId);
		if (q == null)
		{
			return false;
		}
		q.Reload();
		return true;
	}
	
	/**
	 * Unload all quests and scripts and reload them.
	 */
	public void reloadAllScripts()
	{
		unloadAllScripts();
		
		LOGGER.Info("Reloading all server scripts.");
		
		getInstance().report();
	}
	
	/**
	 * Unload all quests and scripts.
	 */
	public void unloadAllScripts()
	{
		LOGGER.Info("Unloading all server scripts.");
		
		// Unload quests.
		foreach (Quest quest in _quests.values())
		{
			if (quest != null)
			{
				quest.unload(false);
			}
		}
		_quests.Clear();
	}
	
	/**
	 * Logs how many quests and scripts are loaded.
	 */
	public void report()
	{
		LOGGER.Info(GetType().Name +": Loaded " + _quests.size() + " quests.");
	}
	
	/**
	 * Calls {@link Quest#onSave()} in all quests and scripts.
	 */
	public void save()
	{
		// Save quests.
		foreach (Quest quest in _quests.values())
		{
			quest.onSave();
		}
	}
	
	/**
	 * Gets a quest by name.<br>
	 * <i>For backwards compatibility, verifies scripts with the given name if the quest is not found.</i>
	 * @param name the quest name
	 * @return the quest
	 */
	public Quest? getQuest(string name)
	{
		return _quests.GetValueOrDefault(name);
	}
	
	/**
	 * Gets a quest by ID.
	 * @param questId the ID of the quest to get
	 * @return if found, the quest, {@code null} otherwise
	 */
	public Quest getQuest(int questId)
	{
		foreach (Quest q in _quests.values())
		{
			if (q.getId() == questId)
			{
				return q;
			}
		}
		return null;
	}
	
	/**
	 * Adds a new quest.
	 * @param quest the quest to be added
	 */
	public void addQuest(Quest quest)
	{
		if (quest == null)
		{
			throw new ArgumentNullException(nameof(quest), "Quest argument cannot be null");
		}
		
		// FIXME: unloading the old quest at this point is a tad too late.
		// the new quest has already initialized itself and read the data, starting
		// an unpredictable number of tasks with that data. The old quest will now
		// save data which will never be read.
		// However, requesting the newQuest to re-read the data is not necessarily a
		// good option, since the newQuest may have already started timers, spawned NPCs
		// or taken any other action which it might re-take by re-reading the data.
		// the current solution properly closes the running tasks of the old quest but
		// ignores the data; perhaps the least of all evils...
		Quest old = _quests.put(quest.Name, quest);
		if (old != null)
		{
			old.Unload();
			LOGGER.Info("Replaced quest " + old.Name + " (" + old.getId() + ") with a new version!");
		}
		
		if (Config.ALT_DEV_SHOW_QUESTS_LOAD_IN_LOGS)
		{
			string questName = quest.Name.Contains("_") ? quest.Name.Substring(quest.Name.IndexOf('_') + 1) : quest.Name;
			LOGGER.Info("Loaded quest " + questName + ".");
		}
	}
	
	/**
	 * Removes a script.
	 * @param script the script to remove
	 * @return {@code true} if the script was removed, {@code false} otherwise
	 */
	public bool removeScript(Quest script)
	{
		if (_quests.ContainsKey(script.Name))
		{
			_quests.remove(script.Name);
			return true;
		}

		return false;
	}
	
	public Map<string, Quest> getQuests()
	{
		return _quests;
	}
	
	public bool unload(Quest ms)
	{
		ms.onSave();
		return removeScript(ms);
	}
	
	/**
	 * Gets the single instance of {@code QuestManager}.
	 * @return single instance of {@code QuestManager}
	 */
	public static QuestManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly QuestManager INSTANCE = new QuestManager();
	}
}