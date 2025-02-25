using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Quests.NewQuestData;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Quests;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Quests;

/**
 * Quest state class.
 * @author Luis Arias
 */
public class QuestState
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(QuestState));

	// Constants
	private const string COND_VAR = "cond";
	private const string COUNT_VAR = "count";
	private const string RESTART_VAR = "restartTime";
	private const string MEMO_VAR = "memoState";
	private const string MEMO_EX_VAR = "memoStateEx";

	/** The name of the quest of this QuestState */
	private readonly string _questName;

	/** The "owner" of this QuestState object */
	private readonly Player _player;

	/** The current state of the quest */
	private byte _state;

	/** The current condition of the quest */
	private QuestCondType _cond = QuestCondType.NONE;

	/** Used for simulating Quest onTalk */
	private bool _simulated;

	/** A map of key=>value pairs containing the quest state variables and their values */
	private Map<string, string> _vars;

	/**
	 * bool flag letting QuestStateManager know to exit quest when cleaning up
	 */
	private bool _isExitQuestOnCleanUp;

	/**
	 * Constructor of the QuestState. Creates the QuestState object and sets the player's progress of the quest to this QuestState.
	 * @param quest the {@link Quest} object associated with the QuestState
	 * @param player the owner of this {@link QuestState} object
	 * @param state the initial state of the quest
	 */
	public QuestState(Quest quest, Player player, byte state)
	{
		_questName = quest.Name;
		_player = player;
		_state = state;
		player.setQuestState(this);
	}

	/**
	 * @return the name of the quest of this QuestState
	 */
	public string getQuestName()
	{
		return _questName;
	}

	/**
	 * @return the {@link Quest} object of this QuestState
	 */
	public Quest getQuest()
	{
		return QuestManager.getInstance().getQuest(_questName);
	}

	/**
	 * @return the {@link Player} object of the owner of this QuestState
	 */
	public Player getPlayer()
	{
		return _player;
	}

	/**
	 * @return the current State of this QuestState
	 * @see org.l2jmobius.gameserver.model.quest.State
	 */
	public byte getState()
	{
		return _state;
	}

	/**
	 * @return {@code true} if the State of this QuestState is CREATED, {@code false} otherwise
	 * @see org.l2jmobius.gameserver.model.quest.State
	 */
	public bool isCreated()
	{
		return _state == State.CREATED;
	}

	/**
	 * @return {@code true} if the State of this QuestState is STARTED, {@code false} otherwise
	 * @see org.l2jmobius.gameserver.model.quest.State
	 */
	public bool isStarted()
	{
		return _state == State.STARTED;
	}

	/**
	 * @return {@code true} if the State of this QuestState is COMPLETED, {@code false} otherwise
	 * @see org.l2jmobius.gameserver.model.quest.State
	 */
	public bool isCompleted()
	{
		return _state == State.COMPLETED;
	}

	/**
	 * @param state the new state of the quest to set
	 * @see #setState(byte state, bool saveInDb)
	 * @see org.l2jmobius.gameserver.model.quest.State
	 */
	public void setState(byte state)
	{
		setState(state, true);
	}

	/**
	 * Change the state of this quest to the specified value.
	 * @param state the new state of the quest to set
	 * @param saveInDb if {@code true}, will save the state change in the database
	 * @see org.l2jmobius.gameserver.model.quest.State
	 */
	public void setState(byte state, bool saveInDb)
	{
		if (_simulated)
		{
			return;
		}

		if (_state == state)
		{
			return;
		}

		bool newQuest = isCreated();
		_state = state;
		if (saveInDb)
		{
			if (newQuest)
			{
				Quest.createQuestInDb(this);
			}
			else
			{
				Quest.updateQuestInDb(this);
			}
		}

		_player.sendPacket(new QuestListPacket(_player));
	}

	/**
	 * Add parameter used in quests.
	 * @param variable String pointing out the name of the variable for quest
	 * @param value String pointing out the value of the variable for quest
	 */
	public void setInternal(string variable, string value)
	{
		if (_simulated)
		{
			return;
		}

		if (_vars == null)
		{
			_vars = new();
		}

		if (value == null)
		{
			_vars.put(variable, "");
			return;
		}

		if (COND_VAR.Equals(variable))
		{
			try
			{
				_cond = (QuestCondType)int.Parse(value);
			}
			catch (Exception ignored)
			{
				// TODO: logging
			}
		}

		_vars.put(variable, value);
	}

	public void set(string variable, int value)
	{
		if (_simulated)
		{
			return;
		}

		set(variable, value.ToString());
	}

	/**
	 * Return value of parameter "value" after adding the couple (var,value) in class variable "vars".<br>
	 * Actions:<br>
	 * <ul>
	 * <li>Initialize class variable "vars" if is null.</li>
	 * <li>Initialize parameter "value" if is null</li>
	 * <li>Add/Update couple (var,value) in class variable Map "vars"</li>
	 * <li>If the key represented by "var" exists in Map "vars", the couple (var,value) is updated in the database.<br>
	 * The key is known as existing if the preceding value of the key (given as result of function put()) is not null.<br>
	 * If the key doesn't exist, the couple is added/created in the database</li>
	 * <ul>
	 * @param variable String indicating the name of the variable for quest
	 * @param value String indicating the value of the variable for quest
	 */
	public void set(string variable, string value)
	{
		if (_simulated)
		{
			return;
		}

		if (_vars == null)
		{
			_vars = new();
		}

		string newValue = value;
		if (newValue == null)
		{
			newValue = "";
		}

		string old = _vars.put(variable, newValue);
		if (old != null)
		{
			Quest.updateQuestVarInDb(this, variable, newValue);
		}
		else
		{
			Quest.createQuestVarInDb(this, variable, newValue);
		}

		if (COND_VAR.Equals(variable))
		{
			try
			{
				int previousVal = 0;
				try
				{
					previousVal = int.Parse(old);
				}
				catch (Exception ignored)
				{
				}
				int newCond = 0;
				try
				{
					newCond = int.Parse(newValue);
				}
				catch (Exception ignored)
				{
				}

				_cond = (QuestCondType)newCond;
				setCond(newCond, previousVal);
				getQuest().sendNpcLogList(getPlayer());
			}
			catch (Exception e)
			{
				LOGGER.Warn(_player.getName() + ", " + _questName + " cond [" + newValue +
				            "] is not an integer.  Value stored, but no packet was sent: " + e);
			}
		}
	}

	/**
	 * Internally handles the progression of the quest so that it is ready for sending appropriate packets to the client.<br>
	 * <u><i>Actions :</i></u><br>
	 * <ul>
	 * <li>Check if the new progress number resets the quest to a previous (smaller) step.</li>
	 * <li>If not, check if quest progress steps have been skipped.</li>
	 * <li>If skipped, prepare the variable completedStateFlags appropriately to be ready for sending to clients.</li>
	 * <li>If no steps were skipped, flags do not need to be prepared...</li>
	 * <li>If the passed step resets the quest to a previous step, reset such that steps after the parameter are not considered, while skipped steps before the parameter, if any, maintain their info.</li>
	 * </ul>
	 * @param cond the current quest progress condition (0 - 31 including)
	 * @param old the previous quest progress condition to check against
	 */
	private void setCond(int cond, int old)
	{
		if (_simulated)
		{
			return;
		}

		if (cond == old)
		{
			return;
		}

		int completedStateFlags = 0;
		// cond 0 and 1 do not need completedStateFlags. Also, if cond > 1, the 1st step must
		// always exist (i.e. it can never be skipped). So if cond is 2, we can still safely
		// assume no steps have been skipped.
		// Finally, more than 31 steps CANNOT be supported in any way with skipping.
		if (cond < 3 || cond > 31)
		{
			unset("__compltdStateFlags");
		}
		else
		{
			completedStateFlags = getInt("__compltdStateFlags");
		}

		// case 1: No steps have been skipped so far...
		if (completedStateFlags == 0)
		{
			// Check if this step also doesn't skip anything. If so, no further work is needed also, in this case, no work is needed if the state is being reset to a smaller value in those cases, skip forward to informing the client about the change...
			// ELSE, if we just now skipped for the first time...prepare the flags!!!
			if (cond > old + 1)
			{
				// set the most significant bit to 1 (indicates that there exist skipped states)
				// also, ensure that the least significant bit is an 1 (the first step is never skipped, no matter what the cond says)
				completedStateFlags = unchecked((int)0x80000001);

				// since no flag had been skipped until now, the least significant bits must all be set to 1, up until "old" number of bits.
				completedStateFlags |= (1 << old) - 1;

				// now, just set the bit corresponding to the passed cond to 1 (current step)
				completedStateFlags |= 1 << (cond - 1);
				set("__compltdStateFlags", completedStateFlags.ToString());
			}
		}
		// case 2: There were exist previously skipped steps
		else if (cond < old) // if this is a push back to a previous step, clear all completion flags ahead
		{
			completedStateFlags &= (1 << cond) - 1; // note, this also unsets the flag indicating that there exist skips

			// now, check if this resulted in no steps being skipped any more
			if (completedStateFlags == (1 << cond) - 1)
			{
				unset("__compltdStateFlags");
			}
			else
			{
				// set the most significant bit back to 1 again, to correctly indicate that this skips states.
				// also, ensure that the least significant bit is an 1 (the first step is never skipped, no matter what the cond says)
				completedStateFlags |= unchecked((int)0x80000001);
				set("__compltdStateFlags", completedStateFlags.ToString());
			}
		}
		// If this moves forward, it changes nothing on previously skipped steps.
		// Just mark this state and we are done.
		else
		{
			completedStateFlags |= 1 << (cond - 1);
			set("__compltdStateFlags", completedStateFlags.ToString());
		}

		// send a packet to the client to inform it of the quest progress (step change)
		_player.sendPacket(new ExQuestUiPacket(_player));
		_player.sendPacket(new ExQuestNotificationAllPacket(_player));
	}

	/**
	 * Removes a quest variable from the list of existing quest variables.
	 * @param variable the name of the variable to remove
	 */
	public void unset(string variable)
	{
		if (_simulated)
		{
			return;
		}

		if (_vars == null)
		{
			return;
		}

		string? old = _vars.remove(variable);
		if (old != null)
		{
			if (COND_VAR.Equals(variable))
			{
				_cond = 0;
			}

			Quest.deleteQuestVarInDb(this, variable);
		}
	}

	/**
	 * @param variable the name of the variable to get
	 * @return the value of the variable from the list of quest variables
	 */
	public string get(string variable)
	{
		if (_vars == null)
		{
			return null;
		}

		return _vars.get(variable);
	}

	/**
	 * @param variable the name of the variable to get
	 * @return the integer value of the variable or 0 if the variable does not exist or its value is not an integer
	 */
	public int getInt(string variable)
	{
		if (_vars == null)
		{
			return 0;
		}

		string? varStr = _vars.get(variable);
		if (string.IsNullOrEmpty(varStr))
		{
			return 0;
		}

		int varInt = 0;
		try
		{
			varInt = int.Parse(varStr);
		}
		catch (FormatException nfe)
		{
			LOGGER.Warn(
				"Quest " + _questName + ", method getInt(" + variable + "), tried to parse a non-integer value (" +
				varStr + "). Char Id: " + _player.ObjectId + ". " + nfe);
		}

		return varInt;
	}

	/**
	 * Checks if the quest state progress ({@code cond}) is at the specified step.
	 * @param condition the condition to check against
	 * @return {@code true} if the quest condition is equal to {@code condition}, {@code false} otherwise
	 * @see #getInt(String var)
	 */
	public bool isCond(QuestCondType condition)
	{
		return _cond == condition;
	}

	/**
	 * Sets the quest state progress ({@code cond}) to the specified step.
	 * @param condition the new condition of the quest state progress
	 * @see #set(String var, String value)
	 * @see #setCond(int, bool)
	 */
	public void setCond(QuestCondType condition)
	{
		if (_simulated)
		{
			return;
		}

		if (isStarted())
		{
			set(COND_VAR, condition.ToString());
			if (condition == QuestCondType.DONE)
			{
				string soundName = QuestSound.ITEMSOUND_QUEST_MIDDLE.GetSoundName();
				_player.sendPacket(new PlaySoundPacket(soundName));
			}
		}
	}

	/**
	 * @return the current quest progress ({@code cond})
	 */
	public QuestCondType getCond()
	{
		if (isStarted())
		{
			return _cond;
		}

		return 0;
	}

	/**
	 * Get bit set representing completed conds.
	 * @return if none cond is set {@code 0}, otherwise cond bit set.
	 */
	public int getCondBitSet()
	{
		if (isStarted())
		{
			int val = getInt(COND_VAR);
			if ((val & 0x80000000) != 0)
			{
				val &= 0x7fffffff;
				for (int i = 1; i < 32; i++)
				{
					val = val >> 1;
					if (val == 0)
					{
						val = i;
						break;
					}
				}
			}
			return val;
		}
		return 0;
	}

	/**
	 * Check if a given variable is set for this quest.
	 * @param variable the variable to check
	 * @return {@code true} if the variable is set, {@code false} otherwise
	 * @see #get(String)
	 * @see #getInt(String)
	 * @see #getCond()
	 */
	public bool isSet(string variable)
	{
		return get(variable) != null;
	}

	/**
	 * Sets the quest state progress ({@code cond}) to the specified step.
	 * @param value the new value of the quest state progress
	 * @param playQuestMiddle if {@code true}, plays "ItemSound.quest_middle"
	 * @see #setCond(int value)
	 * @see #set(String var, String value)
	 */
	public void setCond(int value, bool playQuestMiddle)
	{
		if (_simulated)
		{
			return;
		}

		if (!isStarted())
		{
			return;
		}

		set(COND_VAR, value.ToString());

		if (playQuestMiddle)
		{
			string soundName = QuestSound.ITEMSOUND_QUEST_MIDDLE.GetSoundName();
			_player.sendPacket(new PlaySoundPacket(soundName));
		}
	}

	public void setMemoState(int value)
	{
		if (_simulated)
		{
			return;
		}

		set(MEMO_VAR, value.ToString());
	}

	/**
	 * @return the current Memo State
	 */
	public int getMemoState()
	{
		if (isStarted())
		{
			return getInt(MEMO_VAR);
		}

		return 0;
	}

	public void setCount(int value)
	{
		if (_simulated)
		{
			return;
		}

		set(COUNT_VAR, value.ToString());

		string soundName = QuestSound.ITEMSOUND_QUEST_ITEMGET.GetSoundName();
		_player.sendPacket(new PlaySoundPacket(soundName));

		_player.sendPacket(new ExQuestUiPacket(_player));
		_player.sendPacket(new ExQuestNotificationAllPacket(_player));
	}

	/**
	 * @return the current count
	 */
	public int getCount()
	{
		if (isStarted())
		{
			return getInt(COUNT_VAR);
		}

		return 0;
	}

	public bool isMemoState(int memoState)
	{
		return getInt(MEMO_VAR) == memoState;
	}

	/**
	 * Gets the memo state ex.
	 * @param slot the slot where the value was saved
	 * @return the memo state ex
	 */
	public int getMemoStateEx(int slot)
	{
		if (isStarted())
		{
			return getInt(MEMO_EX_VAR + slot);
		}

		return 0;
	}

	/**
	 * Sets the memo state ex.
	 * @param slot the slot where the value will be saved
	 * @param value the value
	 */
	public void setMemoStateEx(int slot, int value)
	{
		if (_simulated)
		{
			return;
		}

		set(MEMO_EX_VAR + slot, value.ToString());
	}

	/**
	 * Verifies if the given value is equal to the current memos state ex.
	 * @param slot the slot where the value was saved
	 * @param memoStateEx the value to verify
	 * @return {@code true} if the values are equal, {@code false} otherwise
	 */
	public bool isMemoStateEx(int slot, int memoStateEx)
	{
		return getMemoStateEx(slot) == memoStateEx;
	}

	/**
	 * @return {@code true} if quest is to be exited on clean up by QuestStateManager, {@code false} otherwise
	 */
	public bool isExitQuestOnCleanUp()
	{
		return _isExitQuestOnCleanUp;
	}

	/**
	 * @param isExitQuestOnCleanUp {@code true} if quest is to be exited on clean up by QuestStateManager, {@code false} otherwise
	 */
	public void setIsExitQuestOnCleanUp(bool isExitQuestOnCleanUp)
	{
		if (_simulated)
		{
			return;
		}

		_isExitQuestOnCleanUp = isExitQuestOnCleanUp;
	}

	/**
	 * Set condition to 1, state to STARTED and play the "ItemSound.quest_accept".<br>
	 * Works only if state is CREATED and the quest is not a custom quest.
	 */
	public void startQuest()
	{
		if (_simulated)
		{
			return;
		}

		if (isCreated() && !getQuest().isCustomQuest())
		{
			set(COND_VAR, "1");
			set(COUNT_VAR, "0");
			setState(State.STARTED);

			string soundName = QuestSound.ITEMSOUND_QUEST_ACCEPT.GetSoundName();
			_player.sendPacket(new PlaySoundPacket(soundName));
			_player.sendPacket(new ExQuestUiPacket(_player));
			_player.sendPacket(new ExQuestNotificationPacket(this));
			_player.sendPacket(new ExQuestNotificationAllPacket(_player));
			_player.sendPacket(new ExQuestAcceptableListPacket(_player));

			if (_player.Events.HasSubscribers<OnPlayerQuestAccept>())
			{
				_player.Events.NotifyAsync(new OnPlayerQuestAccept(_player, getQuest().getId()));
			}
		}
	}

	/**
	 * Finishes the quest and removes all quest items associated with this quest from the player's inventory.<br>
	 * If {@code type} is {@code QuestType.ONE_TIME}, also removes all other quest data associated with this quest.
	 * @param type the {@link QuestType} of the quest
	 * @see #exitQuest(QuestType type, bool playExitQuest)
	 * @see #exitQuest(bool repeatable)
	 * @see #exitQuest(bool repeatable, bool playExitQuest)
	 */
	public void exitQuest(QuestType type)
	{
		if (_simulated)
		{
			return;
		}

		switch (type)
		{
			case QuestType.DAILY:
			{
				exitQuest(false);
				setRestartTime();
				break;
			}
			// case ONE_TIME:
			// case REPEATABLE:
			default:
			{
				exitQuest(type == QuestType.REPEATABLE);
				break;
			}
		}

		// Notify to scripts
		if (_player.Events.HasSubscribers<OnPlayerQuestComplete>())
		{
			_player.Events.NotifyAsync(new OnPlayerQuestComplete(_player, getQuest().getId(), type));
		}
	}

	/**
	 * Finishes the quest and removes all quest items associated with this quest from the player's inventory.<br>
	 * If {@code type} is {@code QuestType.ONE_TIME}, also removes all other quest data associated with this quest.
	 * @param type the {@link QuestType} of the quest
	 * @param playExitQuest if {@code true}, plays "ItemSound.quest_finish"
	 * @see #exitQuest(QuestType type)
	 * @see #exitQuest(bool repeatable)
	 * @see #exitQuest(bool repeatable, bool playExitQuest)
	 */
	public void exitQuest(QuestType type, bool playExitQuest)
	{
		if (_simulated)
		{
			return;
		}

		exitQuest(type);
		if (playExitQuest)
		{
			string soundName = QuestSound.ITEMSOUND_QUEST_FINISH.GetSoundName();
			_player.sendPacket(new PlaySoundPacket(soundName));
		}

		_player.sendPacket(new ExQuestNotificationAllPacket(getPlayer()));
	}

	/**
	 * Finishes the quest and removes all quest items associated with this quest from the player's inventory.<br>
	 * If {@code repeatable} is set to {@code false}, also removes all other quest data associated with this quest.
	 * @param repeatable if {@code true}, deletes all data and variables of this quest, otherwise keeps them
	 * @see #exitQuest(QuestType type)
	 * @see #exitQuest(QuestType type, bool playExitQuest)
	 * @see #exitQuest(bool repeatable, bool playExitQuest)
	 */
	private void exitQuest(bool repeatable)
	{
		if (_simulated)
		{
			return;
		}

		_player.removeNotifyQuestOfDeath(this);

		if (!isStarted())
		{
			return;
		}

		// Clean registered quest items
		getQuest().removeRegisteredQuestItems(_player);

		Quest.deleteQuestInDb(this, repeatable);
		if (repeatable)
		{
			_player.delQuestState(_questName);
			_player.sendPacket(new ExQuestUiPacket(_player));
		}
		else
		{
			setState(State.COMPLETED);
		}

		_player.sendPacket(new ExQuestNotificationAllPacket(_player));
		_player.sendPacket(new ExQuestUiPacket(_player));

		_vars = null;
	}

	/**
	 * Finishes the quest and removes all quest items associated with this quest from the player's inventory.<br>
	 * If {@code repeatable} is set to {@code false}, also removes all other quest data associated with this quest.
	 * @param repeatable if {@code true}, deletes all data and variables of this quest, otherwise keeps them
	 * @param playExitQuest if {@code true}, plays "ItemSound.quest_finish"
	 * @see #exitQuest(QuestType type)
	 * @see #exitQuest(QuestType type, bool playExitQuest)
	 * @see #exitQuest(bool repeatable)
	 */
	public void exitQuest(bool repeatable, bool playExitQuest)
	{
		if (_simulated)
		{
			return;
		}

		exitQuest(repeatable);
		if (playExitQuest)
		{
			string soundName = QuestSound.ITEMSOUND_QUEST_FINISH.GetSoundName();
			_player.sendPacket(new PlaySoundPacket(soundName));
		}

		_player.sendPacket(new ExQuestNotificationAllPacket(_player));
		_player.sendPacket(new ExQuestUiPacket(_player));

		// Notify to scripts
		if (_player.Events.HasSubscribers<OnPlayerQuestComplete>())
		{
			_player.Events.NotifyAsync(new OnPlayerQuestComplete(_player, getQuest().getId(), repeatable ? QuestType.REPEATABLE : QuestType.ONE_TIME));
		}
	}

	/**
	 * Set the restart time for the daily quests.<br>
	 * The time is hardcoded at {@link Quest#getResetHour()} hours, {@link Quest#getResetMinutes()} minutes of the following day.<br>
	 * It can be overridden in scripts (quests).
	 */
	public void setRestartTime()
	{
		if (_simulated)
		{
			return;
		}

		DateTime reDo = DateTime.Now;
		if (reDo.Hour >= getQuest().getResetHour())
		{
			reDo = reDo.AddDays(1);
		}

		reDo = new DateTime(reDo.Year, reDo.Month, reDo.Day, getQuest().getResetHour(), getQuest().getResetMinutes(), 0);
		set(RESTART_VAR, reDo.Ticks.ToString());
	}

	/**
	 * Check if a daily quest is available to be started over.
	 * @return {@code true} if the quest is available, {@code false} otherwise.
	 */
	public bool isNowAvailable()
	{
		string val = get(RESTART_VAR);
		return val != null && new DateTime(long.Parse(val)) <= DateTime.Now;
	}

	public void setSimulated(bool simulated)
	{
		_simulated = simulated;
	}
}