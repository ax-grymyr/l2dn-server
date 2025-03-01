using System.Globalization;
using L2Dn.Extensions;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Npcs;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Quests.NewQuestData;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.Model.Quests;

/**
 * Quest main class.
 * @author Luis Arias
 */
public class Quest: AbstractScript, IIdentifiable
{
	public static readonly Logger LOGGER = LogManager.GetLogger(nameof(Quest));

	/** Map containing lists of timers from the name of the timer. */
	private readonly Map<string, List<QuestTimer>> _questTimers = new();

	/** Map containing all the start conditions. */
	private readonly Set<QuestCondition> _startCondition = new();

	private readonly int _questId;
	private readonly byte _initialState = State.CREATED;
	private bool _isCustom;
	private NpcStringId _questNameNpcStringId;

	private int[] _questItemIds = [];

	private readonly NewQuest? _questData;

	private const string DEFAULT_NO_QUEST_MSG = "<html><body>You are either not on a quest that involves this NPC, " +
	                                            "or you don't meet this NPC's minimum quest requirements.</body></html>";

	private const int RESET_HOUR = 6;
	private const int RESET_MINUTES = 30;

	private static readonly SkillHolder[] _storyQuestBuffs =
	[
		new SkillHolder(1068, 1), // Might
		new SkillHolder(1040, 1), // Shield
		new SkillHolder(1204, 1), // Wind Walk
		new SkillHolder(1086, 1), // Haste
		new SkillHolder(1085, 1) // Acumen
	];

	/**
	 * @return the reset hour for a daily quest, could be overridden on a script.
	 */
	public int getResetHour()
	{
		return RESET_HOUR;
	}

	/**
	 * @return the reset minutes for a daily quest, could be overridden on a script.
	 */
	public int getResetMinutes()
	{
		return RESET_MINUTES;
	}

	/**
	 * The Quest object constructor.<br>
	 * Constructing a quest also calls the {@code init_LoadGlobalData} convenience method.
	 * @param questId ID of the quest
	 */
	public Quest(int questId)
	{
		_questId = questId;
		// if (questId > 0)
		// {
		// 	QuestManager.getInstance()?.addQuest(this);
		// }
		// else
		// {
		// 	QuestManager.getInstance()?.addScript(this);
		// }

        _questData = Data.Xml.NewQuestData.getInstance().getQuestById(questId);
        if (_questData != null)
        {
            addNewQuestConditions(_questData.getConditions(), null);

            if (_questData.getQuestType() == 1)
            {
                if (_questData.getStartNpcId() > 0)
                {
                    addFirstTalkId(_questData.getStartNpcId());
                }

                if (_questData.getEndNpcId() > 0 && _questData.getEndNpcId() != _questData.getStartNpcId())
                {
                    addFirstTalkId(_questData.getEndNpcId());
                }
            }
            else if (_questData.getQuestType() == 4)
            {
                if (_questData.getStartItemId() > 0)
                {
                    addItemTalkId(_questData.getStartItemId());
                }
            }

            if (_questData.getGoal().getItemId() > 0)
            {
                registerQuestItems(_questData.getGoal().getItemId());
            }
        }

        onLoad();
	}

	/**
	 * This method is, by default, called by the constructor of all scripts.<br>
	 * Children of this class can implement this function in order to define what variables to load and what structures to save them in.<br>
	 * By default, nothing is loaded.
	 */
	protected void onLoad()
	{
	}

	/**
	 * The function onSave is, by default, called at shutdown, for all quests, by the QuestManager.<br>
	 * Children of this class can implement this function in order to convert their structures<br>
	 * into <var, value> tuples and make calls to save them to the database, if needed.<br>
	 * By default, nothing is saved.
	 */
	public void onSave()
	{
	}

	/**
	 * Gets the quest ID.
	 * @return the quest ID
	 */
	public int getId()
	{
		return _questId;
	}

	/**
	 * @return the NpcStringId of the current quest, used in Quest link bypass
	 */
	public int getNpcStringId()
	{
		return _questNameNpcStringId != 0 ? (int)_questNameNpcStringId / 100 : _questId > 10000 ? _questId - 5000 : _questId;
	}

	public NpcStringId getQuestNameNpcStringId()
	{
		return _questNameNpcStringId;
	}

	public void setQuestNameNpcStringId(NpcStringId npcStringId)
	{
		_questNameNpcStringId = npcStringId;
	}

	/**
	 * Add a new quest state of this quest to the database.
	 * @param player the owner of the newly created quest state
	 * @return the newly created {@link QuestState} object
	 */
	public QuestState newQuestState(Player player)
	{
		return new QuestState(this, player, _initialState);
	}

	/**
	 * Get the specified player's {@link QuestState} object for this quest.<br>
	 * If the player does not have it and initIfNode is {@code true},<br>
	 * create a new QuestState object and return it, otherwise return {@code null}.
	 * @param player the player whose QuestState to get
	 * @param initIfNone if true and the player does not have a QuestState for this quest,<br>
	 *            create a new QuestState
	 * @return the QuestState object for this quest or null if it doesn't exist
	 */
	public QuestState? getQuestState(Player player, bool initIfNone)
	{
		QuestState? qs = player.getQuestState(Name);
		if (qs != null || !initIfNone)
		{
			return qs;
		}

		return newQuestState(player);
	}

	/**
	 * @return the initial state of the quest
	 */
	public byte getInitialState()
	{
		return _initialState;
	}
	/**
	 * Send an HTML file to the specified player.
	 * @param player the player to send the HTML file to
	 * @param filename the name of the HTML file to show
	 * @param npc the NPC that is showing the HTML file
	 * @return the contents of the HTML file that was sent to the player
	 * @see #showHtmlFile(Player, String, Npc)
	 */
	public override string? showHtmlFile(Player player, string filename, Npc? npc)
	{
		bool questwindow = !filename.endsWith(".html");

		// Create handler to file linked to the quest
		string content = getHtm(player, filename);

		// Send message to client if message not empty
		if (content != null)
		{
			if (npc != null)
			{
				content = content.Replace("%objectId%", npc.ObjectId.ToString());
			}

			HtmlContent htmlContent = HtmlContent.LoadFromText(content, player);
			htmlContent.Replace("%playername%", player.getName());

			if (questwindow && _questId > 0 && _questId < 20000 && _questId != 999)
			{
				NpcQuestHtmlMessagePacket npcReply = new NpcQuestHtmlMessagePacket(npc?.ObjectId, _questId, htmlContent);
				player.sendPacket(npcReply);
			}
			else
			{
				NpcHtmlMessagePacket npcReply = new NpcHtmlMessagePacket(npc?.ObjectId, 0, htmlContent);
				player.sendPacket(npcReply);
			}

			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		}

		return content;
	}

	/**
	 * Add a timer to the quest (if it doesn't exist already) and start it.
	 * @param name the name of the timer (also passed back as "event" in {@link #onAdvEvent(String, Npc, Player)})
	 * @param time time in ms for when to fire the timer
	 * @param npc the NPC associated with this timer (can be null)
	 * @param player the player associated with this timer (can be null)
	 * @see #startQuestTimer(String, long, Npc, Player, bool)
	 */
	public void startQuestTimer(string name, TimeSpan time, Npc? npc, Player player)
	{
		startQuestTimer(name, time, npc, player, false);
	}

	/**
	 * Gets the quest timers.
	 * @return the quest timers
	 */
	public Map<string, List<QuestTimer>> getQuestTimers()
	{
		return _questTimers;
	}

	/**
	 * Add a timer to the quest (if it doesn't exist already) and start it.
	 * @param name the name of the timer (also passed back as "event" in {@link #onAdvEvent(String, Npc, Player)})
	 * @param time time in ms for when to fire the timer
	 * @param npc the NPC associated with this timer (can be null)
	 * @param player the player associated with this timer (can be null)
	 * @param repeating indicates whether the timer is repeatable or one-time.<br>
	 *            If {@code true}, the task is repeated every {@code time} milliseconds until explicitly stopped.
	 */
	public void startQuestTimer(string name, TimeSpan time, Npc? npc, Player player, bool repeating)
	{
		if (name == null)
		{
			return;
		}

		lock (_questTimers)
        {
            List<QuestTimer> timers = _questTimers.GetOrAdd(name, _ => []);

			// If there exists a timer with this name, allow the timer only if the [npc, player] set is unique nulls act as wildcards.
			if (getQuestTimer(name, npc, player) == null)
				timers.Add(new QuestTimer(this, name, time, npc, player, repeating));
		}
	}

	/**
	 * Get a quest timer that matches the provided name and parameters.
	 * @param name the name of the quest timer to get
	 * @param npc the NPC associated with the quest timer to get
	 * @param player the player associated with the quest timer to get
	 * @return the quest timer that matches the specified parameters or {@code null} if nothing was found
	 */
	public QuestTimer? getQuestTimer(string name, Npc? npc, Player player)
	{
		if (name == null)
		{
			return null;
		}

		List<QuestTimer>? timers = _questTimers.get(name);
		if (timers == null || timers.Count == 0)
		{
			return null;
		}

		foreach (QuestTimer timer in timers)
		{
			if (timer != null && timer.equals(this, name, npc, player))
			{
				return timer;
			}
		}

		return null;
	}

	/**
	 * Cancel all quest timers with the specified name.
	 * @param name the name of the quest timers to cancel
	 */
	public void cancelQuestTimers(string name)
	{
		if (name == null)
		{
			return;
		}

		List<QuestTimer>? timers = _questTimers.get(name);
		if (timers == null || timers.Count == 0)
		{
			return;
		}

		foreach (QuestTimer timer in timers)
		{
			if (timer != null)
			{
				timer.cancel();
			}
		}

		timers.Clear();
	}

	/**
	 * Cancel the quest timer that matches the specified name and parameters.
	 * @param name the name of the quest timer to cancel
	 * @param npc the NPC associated with the quest timer to cancel
	 * @param player the player associated with the quest timer to cancel
	 */
	public void cancelQuestTimer(string name, Npc npc, Player player)
	{
		if (name == null)
		{
			return;
		}

		List<QuestTimer>? timers = _questTimers.get(name);
		if (timers == null || timers.Count == 0)
		{
			return;
		}

		foreach (QuestTimer timer in timers)
		{
			if (timer != null && timer.equals(this, name, npc, player))
			{
				timer.cancel();
			}
		}
	}

	/**
	 * Remove a quest timer from the list of all timers.<br>
	 * Note: does not stop the timer itself!
	 * @param timer the {@link QuestState} object to remove
	 */
	public void removeQuestTimer(QuestTimer timer)
	{
		if (timer == null)
		{
			return;
		}

		List<QuestTimer>? timers = _questTimers.get(timer.ToString());
		if (timers != null)
		{
			timers.Remove(timer);
		}
	}

	// These are methods to call within the core to call the quest events.

	/**
	 * @param npc the NPC that was attacked
	 * @param attacker the attacking player
	 * @param damage the damage dealt to the NPC by the player
	 * @param isSummon if {@code true}, the attack was actually made by the player's summon
	 * @param skill the skill used to attack the NPC (can be null)
	 */
	public void notifyAttack(Npc npc, Player? attacker, int damage, bool isSummon, Skill? skill)
	{
		string? res = null;
		try
		{
			res = onAttack(npc, attacker, damage, isSummon, skill);
		}
		catch (Exception e)
		{
			showError(attacker, e);
			return;
		}

		showResult(attacker, res);
	}

	/**
	 * @param killer the character that killed the {@code victim}
	 * @param victim the character that was killed by the {@code killer}
	 * @param qs the quest state object of the player to be notified of this event
	 */
	public void notifyDeath(Creature killer, Creature victim, QuestState qs)
	{
		string? res = null;
		try
		{
			res = onDeath(killer, victim, qs);
		}
		catch (Exception e)
		{
			showError(qs.getPlayer(), e);
			return;
		}
		showResult(qs.getPlayer(), res);
	}

	/**
	 * @param item
	 * @param player
	 */
	public void notifyItemUse(ItemTemplate item, Player player)
	{
		string? res = null;
		try
		{
			res = onItemUse(item, player);
		}
		catch (Exception e)
		{
			showError(player, e);
			return;
		}
		showResult(player, res);
	}

	/**
	 * @param instance
	 * @param player
	 * @param skill
	 */
	public void notifySpellFinished(Npc instance, Player? player, Skill skill)
	{
		string? res = null;
		try
		{
			res = onSpellFinished(instance, player, skill);
		}
		catch (Exception e)
		{
			showError(player, e);
			return;
		}
		showResult(player, res);
	}

	/**
	 * Notify quest script when something happens with a trap.
	 * @param trap the trap instance which triggers the notification
	 * @param trigger the character which makes effect on the trap
	 * @param action 0: trap casting its skill. 1: trigger detects the trap. 2: trigger removes the trap
	 */
	public void notifyTrapAction(Trap trap, Creature trigger, TrapAction action)
	{
		string? res = null;
		try
		{
			res = onTrapAction(trap, trigger, action);
		}
		catch (Exception e)
		{
			if (trigger.getActingPlayer() != null)
			{
				showError(trigger.getActingPlayer(), e);
			}
			LOGGER.Warn("Exception on onTrapAction() in notifyTrapAction(): " + e);
			return;
		}
		if (trigger.getActingPlayer() is {} player)
		{
			showResult(player, res);
		}
	}

	/**
	 * @param npc the spawned NPC
	 */
	public void notifySpawn(Npc npc)
	{
		try
		{
			onSpawn(npc);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception on onSpawn() in notifySpawn(): " + e);
		}
	}

	/**
	 * @param npc the teleport NPC
	 */
	public void notifyTeleport(Npc npc)
	{
		try
		{
			onTeleport(npc);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception on onTeleport() in notifyTeleport(): " + e);
		}
	}

	/**
	 * @param event
	 * @param npc
	 * @param player
	 */
	public void notifyEvent(string @event, Npc? npc, Player? player)
	{
		string? res;
		try
		{
			// Simulated talk should not exist when event runs.
			if (player != null)
			{
				player.setSimulatedTalking(false);
			}

			res = onAdvEvent(@event, npc, player);
		}
		catch (Exception e)
		{
			showError(player, e);
			return;
		}

		showResult(player, res, npc);
	}

	/**
	 * @param player the player entering the world
	 */
	public void notifyEnterWorld(Player player)
	{
		string? res = null;
		try
		{
			res = onEnterWorld(player);
		}
		catch (Exception e)
		{
			showError(player, e);
			return;
		}
		showResult(player, res);
	}

	/**
	 * @param npc
	 * @param killer
	 * @param isSummon
	 */
	public void notifyKill(Npc npc, Player? killer, bool isSummon)
	{
		string? res = null;
		try
		{
			// Simulated talk should not exist when killing.
			if (killer != null)
			{
				killer.setSimulatedTalking(false);

				QuestState? qs = getQuestState(killer, false);
				if (qs != null)
				{
					qs.setSimulated(false);
				}
			}

			res = onKill(npc, killer, isSummon);
		}
		catch (Exception e)
		{
			showError(killer, e);
			return;
		}
		showResult(killer, res);
	}

	/**
	 * @param npc
	 * @param player
	 */
	public override void notifyTalk(Npc npc, Player player)
	{
		string? res = null;
		try
		{
			if (npc.Events.HasSubscribers<OnNpcQuestStart>())
			{
				OnNpcQuestStart onNpcQuestStart = new OnNpcQuestStart(npc, player);
				if (npc.Events.Notify(onNpcQuestStart) && onNpcQuestStart.Quests.Contains(this))
				{
					string? startConditionHtml = getStartConditionHtml(player, npc);
					if (startConditionHtml != null)
					{
						res = startConditionHtml;
					}
					else
					{
						res = onTalk(npc, player, false);
					}
				}
			}

		}
		catch (Exception e)
		{
			showError(player, e);
			return;
		}

		player.setLastQuestNpcObject(npc.ObjectId);
		showResult(player, res, npc);
	}

	/**
	 * Override the default NPC dialogs when a quest defines this for the given NPC.<br>
	 * Note: If the default html for this npc needs to be shown, onFirstTalk should call npc.showChatWindow(player) and then return null.
	 * @param npc the NPC whose dialogs to override
	 * @param player the player talking to the NPC
	 */
	public void notifyFirstTalk(Npc npc, Player player)
	{
		string? res = null;
		try
		{
			res = onFirstTalk(npc, player);
		}
		catch (Exception e)
		{
			showError(player, e);
			return;
		}

		showResult(player, res, npc);
	}

	/**
	 * Notify the quest engine that an skill has been acquired.
	 * @param npc the NPC
	 * @param player the player
	 * @param skill the skill
	 * @param type the skill learn type
	 */
	public void notifyAcquireSkill(Npc? npc, Player player, Skill skill, AcquireSkillType type)
	{
		string? res;
		try
		{
			res = onAcquireSkill(npc, player, skill, type);
		}
		catch (Exception e)
		{
			showError(player, e);
			return;
		}

		showResult(player, res);
	}

	/**
	 * @param item
	 * @param player
	 */
	public void notifyItemTalk(Item item, Player player)
	{
		string? res = null;
		try
		{
			res = onItemTalk(item, player);
		}
		catch (Exception e)
		{
			showError(player, e);
			return;
		}
		showResult(player, res);
	}

	/**
	 * @param item
	 * @param player
	 * @return
	 */
	public string? onItemTalk(Item item, Player player)
	{
		return null;
	}

	/**
	 * @param item
	 * @param player
	 * @param event
	 */
	public void notifyItemEvent(Item item, Player player, string @event)
	{
		string? res = null;
		try
		{
			res = onItemEvent(item, player, @event);
			if (res != null && (res.equalsIgnoreCase("true") || res.equalsIgnoreCase("false")))
			{
				return;
			}
		}
		catch (Exception e)
		{
			showError(player, e);
			return;
		}
		showResult(player, res);
	}

	/**
	 * @param npc
	 * @param caster
	 * @param skill
	 * @param targets
	 * @param isSummon
	 */
	public void notifySkillSee(Npc npc, Player caster, Skill skill, WorldObject[] targets, bool isSummon)
	{
		string? res = null;
		try
		{
			res = onSkillSee(npc, caster, skill, targets, isSummon);
		}
		catch (Exception e)
		{
			showError(caster, e);
			return;
		}
		showResult(caster, res);
	}

	/**
	 * @param npc
	 * @param caller
	 * @param attacker
	 * @param isSummon
	 */
	public void notifyFactionCall(Npc npc, Npc caller, Player attacker, bool isSummon)
	{
		string? res = null;
		try
		{
			res = onFactionCall(npc, caller, attacker, isSummon);
		}
		catch (Exception e)
		{
			showError(attacker, e);
			return;
		}
		showResult(attacker, res);
	}

	/**
	 * @param npc
	 * @param player
	 * @param isSummon
	 */
	public void notifyAggroRangeEnter(Npc npc, Player player, bool isSummon)
	{
		string? res = null;
		try
		{
			res = onAggroRangeEnter(npc, player, isSummon);
		}
		catch (Exception e)
		{
			showError(player, e);
			return;
		}
		showResult(player, res);
	}

	/**
	 * @param npc the NPC that sees the creature
	 * @param creature the creature seen by the NPC
	 */
	public void notifyCreatureSee(Npc npc, Creature creature)
	{
		Player? player = null;
		if (creature.isPlayer())
		{
			player = creature.getActingPlayer();
		}

		string? res = null;
		try
		{
			res = onCreatureSee(npc, creature);
		}
		catch (Exception e)
		{
			if (player != null)
			{
				showError(player, e);
			}
			return;
		}
		if (player != null)
		{
			showResult(player, res);
		}
	}

	/**
	 * @param eventName - name of event
	 * @param sender - NPC, who sent event
	 * @param receiver - NPC, who received event
	 * @param reference - WorldObject to pass, if needed
	 */
	public void notifyEventReceived(string eventName, Npc sender, Npc receiver, WorldObject reference)
	{
		try
		{
			onEventReceived(eventName, sender, receiver, reference);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception on onEventReceived() in notifyEventReceived(): " + e);
		}
	}

	/**
	 * @param creature
	 * @param zone
	 */
	public void notifyEnterZone(Creature creature, ZoneType zone)
	{
		Player? player = creature.getActingPlayer();
		string? res = null;
		try
		{
			res = onEnterZone(creature, zone);
		}
		catch (Exception e)
		{
			if (player != null)
			{
				showError(player, e);
			}
			return;
		}
		if (player != null)
		{
			showResult(player, res);
		}
	}

	/**
	 * @param creature
	 * @param zone
	 */
	public void notifyExitZone(Creature creature, ZoneType zone)
	{
		Player? player = creature.getActingPlayer();
		string? res = null;
		try
		{
			res = onExitZone(creature, zone);
		}
		catch (Exception e)
		{
			if (player != null)
			{
				showError(player, e);
			}
			return;
		}
		if (player != null)
		{
			showResult(player, res);
		}
	}

	/**
	 * @param winner
	 * @param looser
	 */
	public void notifyOlympiadMatch(Participant? winner, Participant looser)
	{
		try
		{
			onOlympiadMatchFinish(winner, looser);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Execution on onOlympiadMatchFinish() in notifyOlympiadMatch(): " + e);
		}
	}

	/**
	 * @param npc
	 */
	public void notifyMoveFinished(Npc npc)
	{
		try
		{
			onMoveFinished(npc);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception on onMoveFinished() in notifyMoveFinished(): " + e);
		}
	}

	/**
	 * @param npc
	 */
	public void notifyRouteFinished(Npc npc)
	{
		try
		{
			onRouteFinished(npc);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception on onRouteFinished() in notifyRouteFinished(): " + e);
		}
	}

	/**
	 * @param npc
	 * @param player
	 * @return {@code true} if player can see this npc, {@code false} otherwise.
	 */
	public bool notifyOnCanSeeMe(Npc npc, Player player)
	{
		try
		{
			return onCanSeeMe(npc, player);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception on onCanSeeMe() in notifyOnCanSeeMe(): " + e);
		}
		return false;
	}

	// These are methods that java calls to invoke scripts.

	/**
	 * This function is called in place of {@link #onAttack(Npc, Player, int, bool, Skill)} if the former is not implemented.<br>
	 * If a script contains both onAttack(..) implementations, then this method will never be called unless the script's {@link #onAttack(Npc, Player, int, bool, Skill)} explicitly calls this method.
	 * @param npc this parameter contains a reference to the exact instance of the NPC that got attacked the NPC.
	 * @param attacker this parameter contains a reference to the exact instance of the player who attacked.
	 * @param damage this parameter represents the total damage that this attack has inflicted to the NPC.
	 * @param isSummon this parameter if it's {@code false} it denotes that the attacker was indeed the player, else it specifies that the damage was actually dealt by the player's pet.
	 * @return
	 */
	public string? onAttack(Npc npc, Player? attacker, int damage, bool isSummon)
	{
		return null;
	}

	/**
	 * This function is called whenever a player attacks an NPC that is registered for the quest.<br>
	 * If is not overridden by a subclass, then default to the returned value of the simpler (and older) {@link #onAttack(Npc, Player, int, bool)} override.
	 * @param npc this parameter contains a reference to the exact instance of the NPC that got attacked.
	 * @param attacker this parameter contains a reference to the exact instance of the player who attacked the NPC.
	 * @param damage this parameter represents the total damage that this attack has inflicted to the NPC.
	 * @param isSummon this parameter if it's {@code false} it denotes that the attacker was indeed the player, else it specifies that the damage was actually dealt by the player's summon
	 * @param skill parameter is the skill that player used to attack NPC.
	 * @return
	 */
	public string? onAttack(Npc npc, Player? attacker, int damage, bool isSummon, Skill? skill)
	{
		return onAttack(npc, attacker, damage, isSummon);
	}

	/**
	 * This function is called whenever an <b>exact instance</b> of a character who was previously registered for this event dies.<br>
	 * The registration for {@link #onDeath(Creature, Creature, QuestState)} events <b>is not</b> done via the quest itself, but it is instead handled by the QuestState of a particular player.
	 * @param killer this parameter contains a reference to the exact instance of the NPC that <b>killed</b> the character.
	 * @param victim this parameter contains a reference to the exact instance of the character that got killed.
	 * @param qs this parameter contains a reference to the QuestState of whomever was interested (waiting) for this kill.
	 * @return
	 */
	public string? onDeath(Creature killer, Creature victim, QuestState qs)
	{
		return onAdvEvent("", killer is Npc ? (Npc) killer : null, qs.getPlayer());
	}

	/**
	 * This function is called whenever a player clicks on a link in a quest dialog and whenever a timer fires.<br>
	 * If is not overridden by a subclass, then default to the returned value of the simpler (and older) {@link #onEvent(String, QuestState)} override.<br>
	 * If the player has a quest state, use it as parameter in the next call, otherwise return null.
	 * @param event this parameter contains a string identifier for the event.<br>
	 *            Generally, this string is passed directly via the link.<br>
	 *            For example:<br>
	 *            <code>
	 *            &lt;a action="bypass -h Quest 626_ADarkTwilight 31517-01.htm"&gt;hello&lt;/a&gt;
	 *            </code><br>
	 *            The above link sets the event variable to "31517-01.htm" for the quest 626_ADarkTwilight.<br>
	 *            In the case of timers, this will be the name of the timer.<br>
	 *            This parameter serves as a sort of identifier.
	 * @param npc this parameter contains a reference to the instance of NPC associated with this event.<br>
	 *            This may be the NPC registered in a timer, or the NPC with whom a player is speaking, etc.<br>
	 *            This parameter may be {@code null} in certain circumstances.
	 * @param player this parameter contains a reference to the player participating in this function.<br>
	 *            It may be the player speaking to the NPC, or the player who caused a timer to start (and owns that timer).<br>
	 *            This parameter may be {@code null} in certain circumstances.
	 * @return the text returned by the event (may be {@code null}, a filename or just text)
	 */
	public virtual string? onAdvEvent(string @event, Npc? npc, Player? player)
	{
		if (player != null)
		{
			QuestState? qs = player.getQuestState(Name);
			if (qs != null)
			{
				return onEvent(@event, qs);
			}
		}

		return null;
	}

	/**
	 * This function is called in place of {@link #onAdvEvent(String, Npc, Player)} if the former is not implemented.<br>
	 * If a script contains both {@link #onAdvEvent(String, Npc, Player)} and this implementation, then this method will never be called unless the script's {@link #onAdvEvent(String, Npc, Player)} explicitly calls this method.
	 * @param event this parameter contains a string identifier for the event.<br>
	 *            Generally, this string is passed directly via the link.<br>
	 *            For example:<br>
	 *            <code>
	 *            &lt;a action="bypass -h Quest 626_ADarkTwilight 31517-01.htm"&gt;hello&lt;/a&gt;
	 *            </code><br>
	 *            The above link sets the event variable to "31517-01.htm" for the quest 626_ADarkTwilight.<br>
	 *            In the case of timers, this will be the name of the timer.<br>
	 *            This parameter serves as a sort of identifier.
	 * @param qs this parameter contains a reference to the quest state of the player who used the link or started the timer.
	 * @return the text returned by the event (may be {@code null}, a filename or just text)
	 */
	public string? onEvent(string @event, QuestState qs)
	{
		return null;
	}

	/**
	 * This function is called whenever a player kills a NPC that is registered for the quest.
	 * @param npc this parameter contains a reference to the exact instance of the NPC that got killed.
	 * @param killer this parameter contains a reference to the exact instance of the player who killed the NPC.
	 * @param isSummon this parameter if it's {@code false} it denotes that the attacker was indeed the player, else it specifies that the killer was the player's pet.
	 * @return the text returned by the event (may be {@code null}, a filename or just text)
	 */
	public virtual string? onKill(Npc npc, Player? killer, bool isSummon)
	{
		if (killer != null && !getNpcLogList(killer).isEmpty())
		{
			sendNpcLogList(killer);
		}
		return null;
	}

	/**
	 * This function is called whenever a player clicks to the "Quest" link of an NPC that is registered for the quest.
	 * @param npc this parameter contains a reference to the exact instance of the NPC that the player is talking with.
	 * @param talker this parameter contains a reference to the exact instance of the player who is talking to the NPC.
	 * @param simulated Used by QuestLink to determine state of quest.
	 * @return the text returned by the event (may be {@code null}, a filename or just text)
	 */
	public string? onTalk(Npc npc, Player talker, bool simulated)
	{
		QuestState? qs = talker.getQuestState(Name);
		if (qs != null)
		{
			qs.setSimulated(simulated);
		}
		talker.setSimulatedTalking(simulated);
		return onTalk(npc, talker);
	}

	/**
	 * This function is called whenever a player talks to an NPC that is registered for the quest.<br>
	 * That is, it is triggered from the very first click on the NPC, not via another dialog.<br>
	 * <b>Note 1:</b><br>
	 * Each NPC can be registered to at most one quest for triggering this function.<br>
	 * In other words, the same one NPC cannot respond to an "onFirstTalk" request from two different quests.<br>
	 * Attempting to register an NPC in two different quests for this function will result in one of the two registration being ignored.<br>
	 * <b>Note 2:</b><br>
	 * Since a Quest link isn't clicked in order to reach this, a quest state can be invalid within this function.<br>
	 * The coder of the script may need to create a new quest state (if necessary).<br>
	 * <b>Note 3:</b><br>
	 * The returned value of onFirstTalk replaces the default HTML that would have otherwise been loaded from a sub-folder of DatapackRoot/game/data/html/.<br>
	 * If you wish to show the default HTML, within onFirstTalk do npc.showChatWindow(player) and then return ""
	 * @param npc this parameter contains a reference to the exact instance of the NPC that the player is talking with.
	 * @param player this parameter contains a reference to the exact instance of the player who is talking to the NPC.
	 * @return the text returned by the event (may be {@code null}, a filename or just text)
	 */
	public virtual string? onFirstTalk(Npc npc, Player player)
	{
		return null;
	}

	/**
	 * @param item
	 * @param player
	 * @param event
	 * @return
	 */
	public string? onItemEvent(Item item, Player player, string @event)
	{
		return null;
	}

	/**
	 * This function is called whenever a player request a skill list.<br>
	 * TODO: Re-implement, since Skill Trees rework it's support was removed.
	 * @param npc this parameter contains a reference to the exact instance of the NPC that the player requested the skill list.
	 * @param player this parameter contains a reference to the exact instance of the player who requested the skill list.
	 * @return
	 */
	public string? onAcquireSkillList(Npc npc, Player player)
	{
		return null;
	}

	/**
	 * This function is called whenever a player request a skill info.
	 * @param npc this parameter contains a reference to the exact instance of the NPC that the player requested the skill info.
	 * @param player this parameter contains a reference to the exact instance of the player who requested the skill info.
	 * @param skill this parameter contains a reference to the skill that the player requested its info.
	 * @return
	 */
	public string? onAcquireSkillInfo(Npc npc, Player player, Skill skill)
	{
		return null;
	}

	/**
	 * This function is called whenever a player acquire a skill.<br>
	 * TODO: Re-implement, since Skill Trees rework it's support was removed.
	 * @param npc this parameter contains a reference to the exact instance of the NPC that the player requested the skill.
	 * @param player this parameter contains a reference to the exact instance of the player who requested the skill.
	 * @param skill this parameter contains a reference to the skill that the player requested.
	 * @param type the skill learn type
	 * @return
	 */
	public string? onAcquireSkill(Npc? npc, Player player, Skill skill, AcquireSkillType type)
	{
		return null;
	}

	/**
	 * This function is called whenever a player uses a quest item that has a quest events list.<br>
	 * TODO: complete this documentation and unhardcode it to work with all item uses not with those listed.
	 * @param item the quest item that the player used
	 * @param player the player who used the item
	 * @return
	 */
	public string? onItemUse(ItemTemplate item, Player player)
	{
		return null;
	}

	/**
	 * This function is called whenever a player casts a skill near a registered NPC (1000 distance).<br>
	 * <b>Note:</b><br>
	 * If a skill does damage, both onSkillSee(..) and onAttack(..) will be triggered for the damaged NPC!<br>
	 * However, only onSkillSee(..) will be triggered if the skill does no damage,<br>
	 * or if it damages an NPC who has no onAttack(..) registration while near another NPC who has an onSkillSee registration.<br>
	 * TODO: confirm if the distance is 1000 and unhardcode.
	 * @param npc the NPC that saw the skill
	 * @param caster the player who cast the skill
	 * @param skill the actual skill that was used
	 * @param targets an array of all objects (can be any type of object, including mobs and players) that were affected by the skill
	 * @param isSummon if {@code true}, the skill was actually cast by the player's summon, not the player himself
	 * @return
	 */
	public string? onSkillSee(Npc npc, Player caster, Skill skill, WorldObject[] targets, bool isSummon)
	{
		return null;
	}

	/**
	 * This function is called whenever an NPC finishes casting a skill.
	 * @param npc the NPC that casted the skill.
	 * @param player the player who is the target of the skill. Can be {@code null}.
	 * @param skill the actual skill that was used by the NPC.
	 * @return
	 */
	public string? onSpellFinished(Npc npc, Player? player, Skill skill)
	{
		return null;
	}

	/**
	 * This function is called whenever a trap action is performed.
	 * @param trap this parameter contains a reference to the exact instance of the trap that was activated.
	 * @param trigger this parameter contains a reference to the exact instance of the character that triggered the action.
	 * @param action this parameter contains a reference to the action that was triggered.
	 * @return
	 */
	public string? onTrapAction(Trap trap, Creature trigger, TrapAction action)
	{
		return null;
	}

	/**
	 * This function is called whenever an NPC spawns or re-spawns and passes a reference to the newly (re)spawned NPC.<br>
	 * Currently the only function that has no reference to a player.<br>
	 * It is useful for initializations, starting quest timers, displaying chat (NpcSay), and more.
	 * @param npc this parameter contains a reference to the exact instance of the NPC who just (re)spawned.
	 * @return
	 */
	public string? onSpawn(Npc npc)
	{
		return null;
	}

	/**
	 * This function is called whenever an NPC is teleport.
	 * @param npc this parameter contains a reference to the exact instance of the NPC who just teleport.
	 */
	protected void onTeleport(Npc npc)
	{
	}

	/**
	 * This function is called whenever an NPC is called by another NPC in the same faction.
	 * @param npc this parameter contains a reference to the exact instance of the NPC who is being asked for help.
	 * @param caller this parameter contains a reference to the exact instance of the NPC who is asking for help.
	 * @param attacker this parameter contains a reference to the exact instance of the player who attacked.
	 * @param isSummon this parameter if it's {@code false} it denotes that the attacker was indeed the player, else it specifies that the attacker was the player's summon.
	 * @return
	 */
	public string? onFactionCall(Npc npc, Npc caller, Player attacker, bool isSummon)
	{
		return null;
	}

	/**
	 * This function is called whenever a player enters an NPC aggression range.
	 * @param npc this parameter contains a reference to the exact instance of the NPC whose aggression range is being transgressed.
	 * @param player this parameter contains a reference to the exact instance of the player who is entering the NPC's aggression range.
	 * @param isSummon this parameter if it's {@code false} it denotes that the character that entered the aggression range was indeed the player, else it specifies that the character was the player's summon.
	 * @return
	 */
	public string? onAggroRangeEnter(Npc npc, Player player, bool isSummon)
	{
		return null;
	}

	/**
	 * This function is called whenever an NPC "sees" a creature.
	 * @param npc the NPC who sees the creature
	 * @param creature the creature seen by the NPC
	 * @return
	 */
	public string? onCreatureSee(Npc npc, Creature creature)
	{
		return null;
	}

	/**
	 * This function is called whenever a player enters the game.
	 * @param player this parameter contains a reference to the exact instance of the player who is entering to the world.
	 * @return
	 */
	public string? onEnterWorld(Player player)
	{
		return null;
	}

	/**
	 * This function is called whenever a character enters a registered zone.
	 * @param creature this parameter contains a reference to the exact instance of the character who is entering the zone.
	 * @param zone this parameter contains a reference to the zone.
	 * @return
	 */
	public string? onEnterZone(Creature creature, ZoneType zone)
	{
		return null;
	}

	/**
	 * This function is called whenever a character exits a registered zone.
	 * @param creature this parameter contains a reference to the exact instance of the character who is exiting the zone.
	 * @param zone this parameter contains a reference to the zone.
	 * @return
	 */
	public string? onExitZone(Creature creature, ZoneType zone)
	{
		return null;
	}

	/**
	 * @param eventName - name of event
	 * @param sender - NPC, who sent event
	 * @param receiver - NPC, who received event
	 * @param reference - WorldObject to pass, if needed
	 * @return
	 */
	public string? onEventReceived(string eventName, Npc sender, Npc receiver, WorldObject reference)
	{
		return null;
	}

	/**
	 * This function is called whenever a player wins an Olympiad Game.
	 * @param winner in this match.
	 * @param looser in this match.
	 */
	public void onOlympiadMatchFinish(Participant? winner, Participant looser)
	{
	}

	/**
	 * This function is called whenever a player looses an Olympiad Game.
	 * @param loser this parameter contains a reference to the exact instance of the player who lose the competition.
	 */
	public void onOlympiadLose(Player loser)
	{
	}

	/**
	 * This function is called whenever a NPC finishes moving
	 * @param npc registered NPC
	 */
	public void onMoveFinished(Npc npc)
	{
	}

	/**
	 * This function is called whenever a walker NPC (controlled by WalkingManager) arrive to last node
	 * @param npc registered NPC
	 */
	public void onRouteFinished(Npc npc)
	{
	}

	/**
	 * @param mob
	 * @param player
	 * @param isSummon
	 * @return {@code true} if npc can hate the playable, {@code false} otherwise.
	 */
	public bool onNpcHate(Attackable mob, Player player, bool isSummon)
	{
		return true;
	}

	/**
	 * @param summon
	 */
	public void onSummonSpawn(Summon summon)
	{
	}

	/**
	 * @param summon
	 */
	public void onSummonTalk(Summon summon)
	{
	}

	/**
	 * This listener is called when instance world is created.
	 * @param instance created instance world
	 * @param player player who create instance world
	 */
	public void onInstanceCreated(Instance instance, Player? player)
	{
	}

	/**
	 * This listener is called when instance being destroyed.
	 * @param instance instance world which will be destroyed
	 */
	public void onInstanceDestroy(Instance instance)
	{
	}

	/**
	 * This listener is called when player enter into instance.
	 * @param player player who enter
	 * @param instance instance where player enter
	 */
	public void onInstanceEnter(Player player, Instance instance)
	{
	}

	/**
	 * This listener is called when player leave instance.
	 * @param player player who leaved
	 * @param instance instance which player leaved
	 */
	public void onInstanceLeave(Player player, Instance instance)
	{
	}

	/**
	 * This listener is called when NPC {@code npc} being despawned.
	 * @param npc NPC which will be despawned
	 */
	public void onNpcDespawn(Npc npc)
	{
	}

	/**
	 * @param npc
	 * @param player
	 * @return {@code true} if player can see this npc, {@code false} otherwise.
	 */
	public bool onCanSeeMe(Npc npc, Player player)
	{
		return false;
	}

	/**
	 * Loads all quest states and variables for the specified player.
	 * @param player the player who is entering the world
	 */
	public static void playerEnter(Player player)
	{
		try
		{
			int characterId = player.ObjectId;
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			// Get list of quests owned by the player from database
			var query = from q in ctx.CharacterQuests
				where q.CharacterId == characterId && q.Variable == "<state>"
				select new
				{
					q.Name,
					q.Value
				};

			foreach (var record in query)
			{
				// Get the ID of the quest and its state
				string questId = record.Name;
				string statename = record.Value;

				// Search quest associated with the ID
				Quest? q = QuestManager.getInstance().getQuest(questId);
				if (q == null)
				{
					LOGGER.Warn("Unknown quest " + questId + " for " + player);
					if (Config.AUTODELETE_INVALID_QUEST_DATA)
					{
						ctx.CharacterQuests.Where(r => r.CharacterId == characterId && r.Name == questId)
							.ExecuteDelete();
					}

					continue;
				}

				// Create a new QuestState for the player that will be added to the player's list of quests
				new QuestState(q, player, State.getStateId(statename));
			}

			// Get list of quests owned by the player from the DB in order to add variables used in the quest.
			var query2 = from q in ctx.CharacterQuests
				where q.CharacterId == characterId && q.Variable != "<state>"
				select new
				{
					q.Name,
					q.Variable,
					q.Value
				};

			foreach (var record in query2)
			{
				string questId = record.Name;
				string var = record.Variable;
				string value = record.Value;
				// Get the QuestState saved in the loop before
				QuestState? qs = player.getQuestState(questId);
				if (qs == null)
				{
					LOGGER.Warn("Lost variable " + var + " in quest " + questId + " for " + player);
					if (Config.AUTODELETE_INVALID_QUEST_DATA)
					{
						ctx.CharacterQuests.Where(r =>
								r.CharacterId == characterId && r.Name == questId && r.Variable == var)
							.ExecuteDelete();
					}

					continue;
				}

				// Add parameter to the quest
				qs.setInternal(var, value);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("could not insert char quest: " + e);
		}
	}

	/**
	 * Insert in the database the quest for the player.
	 * @param qs the {@link QuestState} object whose variable to insert
	 * @param var the name of the variable
	 * @param value the value of the variable
	 */
	public static void createQuestVarInDb(QuestState qs, string var, string value)
	{
		try
		{
			int characterId = qs.getPlayer().ObjectId;
			string questName = qs.getQuestName();

			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var record = ctx.CharacterQuests.SingleOrDefault(r =>
				r.CharacterId == characterId && r.Name == questName && r.Variable == var);

			if (record is null)
			{
				record = new CharacterQuest();
				record.CharacterId = characterId;
				record.Name = questName;
				record.Variable = var;
				ctx.CharacterQuests.Add(record);
			}

			record.Value = value;
			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("could not insert char quest: " + e);
		}
	}

	/**
	 * Update the value of the variable "var" for the specified quest in database
	 * @param qs the {@link QuestState} object whose variable to update
	 * @param var the name of the variable
	 * @param value the value of the variable
	 */
	public static void updateQuestVarInDb(QuestState qs, string var, string value)
	{
		createQuestVarInDb(qs, var, value);
	}

	/**
	 * Delete a variable of player's quest from the database.
	 * @param qs the {@link QuestState} object whose variable to delete
	 * @param var the name of the variable to delete
	 */
	public static void deleteQuestVarInDb(QuestState qs, string var)
	{
		try
		{
			int characterId = qs.getPlayer().ObjectId;
			string questName = qs.getQuestName();

			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterQuests.Where(r => r.CharacterId == characterId && r.Name == questName && r.Variable == var)
				.ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Unable to delete char quest: " + e);
		}
	}

	/**
	 * Delete from the database all variables and states of the specified quest state.
	 * @param qs the {@link QuestState} object whose variables to delete
	 * @param repeatable if {@code false}, the state variable will be preserved, otherwise it will be deleted as well
	 */
	public static void deleteQuestInDb(QuestState qs, bool repeatable)
	{
		try
		{
			int characterId = qs.getPlayer().ObjectId;
			string questName = qs.getQuestName();
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			IQueryable<CharacterQuest> query =
				ctx.CharacterQuests.Where(r => r.CharacterId == characterId && r.Name == questName);

			if (!repeatable)
				query = query.Where(r => r.Variable != "<state>");

			query.ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("could not delete char quest: " + e);
		}
	}

	/**
	 * Create a database record for the specified quest state.
	 * @param qs the {@link QuestState} object whose data to write in the database
	 */
	public static void createQuestInDb(QuestState qs)
	{
		createQuestVarInDb(qs, "<state>", State.getStateName(qs.getState()));
	}

	/**
	 * Update a quest state record of the specified quest state in database.
	 * @param qs the {@link QuestState} object whose data to update in the database
	 */
	public static void updateQuestInDb(QuestState qs)
	{
		updateQuestVarInDb(qs, "<state>", State.getStateName(qs.getState()));
	}

	/**
	 * @param player the player whose language settings to use in finding the html of the right language
	 * @return the default html for when no quest is available: "You are either not on a quest that involves this NPC.."
	 */
	public static string getNoQuestMsg(Player player)
	{
		string result = HtmCache.getInstance().getHtm("html/noquest.htm", player.getLang());
		if (result != null && result.Length > 0)
		{
			return result;
		}
		return DEFAULT_NO_QUEST_MSG;
	}

	/**
	 * @param player the player whose language settings to use in finding the html of the right language
	 * @return the default html for when player don't have minimum level for reward: "You cannot receive quest rewards as your character.."
	 */
	public static string getNoQuestLevelRewardMsg(Player player)
	{
		return HtmCache.getInstance().getHtm("html/noquestlevelreward.html", player.getLang());
	}

	/**
	 * @param player the player whose language settings to use in finding the html of the right language
	 * @return the default html for when quest is already completed
	 */
	public static string getAlreadyCompletedMsg(Player player)
	{
		return getAlreadyCompletedMsg(player, QuestType.ONE_TIME);
	}

	/**
	 * @param player the player whose language settings to use in finding the html of the right language
	 * @param type the Quest type
	 * @return the default html for when quest is already completed
	 */
	public static string getAlreadyCompletedMsg(Player player, QuestType type)
	{
		return HtmCache.getInstance()
			.getHtm(
				type == QuestType.ONE_TIME
					? "html/alreadyCompleted.html"
					: "html/alreadyCompletedDaily.html", player.getLang());
	}

	// TODO: Clean up these methods
	public void addStartNpc(int npcId)
	{
		setNpcQuestStartId(ev => ev.Quests.Add(this), npcId);
	}

	public void addFirstTalkId(int npcId)
	{
		setNpcFirstTalkId(@event => notifyFirstTalk(@event.getNpc(), @event.getActiveChar()), npcId);
	}

	public void addKillId(int npcId)
	{
		setAttackableKillId(kill => notifyKill(kill.getTarget(), kill.getAttacker(), kill.isSummon()), npcId);
	}

	public void addAttackId(int npcId)
	{
		setAttackableAttackId(attack => notifyAttack(attack.getTarget(), attack.getAttacker(), attack.getDamage(), attack.isSummon(), attack.getSkill()), npcId);
	}

	/**
	 * Add the quest to the NPC's startQuest
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addStartNpc(params int[] npcIds)
	{
		setNpcQuestStartId(ev => ev.Quests.Add(this), npcIds);
	}

	/**
	 * Add the quest to the NPC's startQuest
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addStartNpc(IReadOnlyCollection<int> npcIds)
	{
		setNpcQuestStartId(ev => ev.Quests.Add(this), npcIds);
	}

	/**
	 * Add the quest to the NPC's first-talk (default action dialog).
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addFirstTalkId(params int[] npcIds)
	{
		setNpcFirstTalkId(@event => notifyFirstTalk(@event.getNpc(), @event.getActiveChar()), npcIds);
	}

	/**
	 * Add the quest to the NPC's first-talk (default action dialog).
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addFirstTalkId(IReadOnlyCollection<int> npcIds)
	{
		setNpcFirstTalkId(@event => notifyFirstTalk(@event.getNpc(), @event.getActiveChar()), npcIds);
	}

	/**
	 * Add the NPC to the AcquireSkill dialog.
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addAcquireSkillId(params int[] npcIds)
	{
		setPlayerSkillLearnId(@event => notifyAcquireSkill(@event.getTrainer(), @event.getPlayer(), @event.getSkill(), @event.getAcquireType()), npcIds);
	}

	/**
	 * Add the NPC to the AcquireSkill dialog.
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addAcquireSkillId(IReadOnlyCollection<int> npcIds)
	{
		setPlayerSkillLearnId(@event => notifyAcquireSkill(@event.getTrainer(), @event.getPlayer(), @event.getSkill(), @event.getAcquireType()), npcIds);
	}

	/**
	 * Add the Item to the notify when player speaks with it.
	 * @param itemIds the IDs of the Item to register
	 */
	public void addItemBypassEventId(params int[] itemIds)
	{
		setItemBypassEvenId(@event => notifyItemEvent(@event.getItem(), @event.getActiveChar(), @event.getEvent()), itemIds);
	}

	/**
	 * Add the Item to the notify when player speaks with it.
	 * @param itemIds the IDs of the Item to register
	 */
	public void addItemBypassEventId(IReadOnlyCollection<int> itemIds)
	{
		setItemBypassEvenId(@event => notifyItemEvent(@event.getItem(), @event.getActiveChar(), @event.getEvent()), itemIds);
	}

	/**
	 * Add the Item to the notify when player speaks with it.
	 * @param itemIds the IDs of the Item to register
	 */
	public void addItemTalkId(params int[] itemIds)
	{
		setItemTalkId(@event => notifyItemTalk(@event.getItem(), @event.getActiveChar()), itemIds);
	}

	/**
	 * Add the Item to the notify when player speaks with it.
	 * @param itemIds the IDs of the Item to register
	 */
	public void addItemTalkId(IReadOnlyCollection<int> itemIds)
	{
		setItemTalkId(@event => notifyItemTalk(@event.getItem(), @event.getActiveChar()), itemIds);
	}

	/**
	 * Add this quest to the list of quests that the passed mob will respond to for attack events.
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addAttackId(params int[] npcIds)
	{
		setAttackableAttackId(attack => notifyAttack(attack.getTarget(), attack.getAttacker(), attack.getDamage(), attack.isSummon(), attack.getSkill()), npcIds);
	}

	/**
	 * Add this quest to the list of quests that the passed mob will respond to for attack events.
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addAttackId(IReadOnlyCollection<int> npcIds)
	{
		setAttackableAttackId(attack => notifyAttack(attack.getTarget(), attack.getAttacker(), attack.getDamage(), attack.isSummon(), attack.getSkill()), npcIds);
	}

	/**
	 * Add this quest to the list of quests that the passed mob will respond to for kill events.
	 * @param npcIds
	 */
	public void addKillId(params int[] npcIds)
	{
		setAttackableKillId(kill => notifyKill(kill.getTarget(), kill.getAttacker(), kill.isSummon()), npcIds);
	}

	/**
	 * Add this quest event to the collection of NPC IDs that will respond to for on kill events.
	 * @param npcIds the collection of NPC IDs
	 */
	public void addKillId(IReadOnlyCollection<int> npcIds)
	{
		setAttackableKillId(kill => notifyKill(kill.getTarget(), kill.getAttacker(), kill.isSummon()), npcIds);
	}

	/**
	 * Add this quest to the list of quests that the passed npc will respond to for Teleport Events.
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addTeleportId(params int[] npcIds)
	{
		setNpcTeleportId(@event => notifyTeleport(@event.getNpc()), npcIds);
	}

	/**
	 * Add this quest to the list of quests that the passed npc will respond to for Teleport Events.
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addTeleportId(IReadOnlyCollection<int> npcIds)
	{
		setNpcTeleportId(@event => notifyTeleport(@event.getNpc()), npcIds);
	}

	/**
	 * Add this quest to the list of quests that the passed npc will respond to for spawn events.
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addSpawnId(params int[] npcIds)
	{
		setNpcSpawnId(@event => notifySpawn(@event.getNpc()), npcIds);
	}

	/**
	 * Add this quest to the list of quests that the passed npc will respond to for spawn events.
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addSpawnId(IReadOnlyCollection<int> npcIds)
	{
		setNpcSpawnId(@event => notifySpawn(@event.getNpc()), npcIds);
	}

	/**
	 * Register onNpcDespawn to NPCs.
	 * @param npcIds
	 */
	public void addDespawnId(params int[] npcIds)
	{
		setNpcDespawnId(@event => onNpcDespawn(@event.getNpc()), npcIds);
	}

	/**
	 * Register onNpcDespawn to NPCs.
	 * @param npcIds
	 */
	public void addDespawnId(IReadOnlyCollection<int> npcIds)
	{
		setNpcDespawnId(@event => onNpcDespawn(@event.getNpc()), npcIds);
	}

	/**
	 * Add this quest to the list of quests that the passed npc will respond to for skill see events.
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addSkillSeeId(params int[] npcIds)
	{
		setNpcSkillSeeId(@event => notifySkillSee(@event.getTarget(), @event.getCaster(), @event.getSkill(), @event.getTargets(), @event.isSummon()), npcIds);
	}

	/**
	 * Add this quest to the list of quests that the passed npc will respond to for skill see events.
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addSkillSeeId(IReadOnlyCollection<int> npcIds)
	{
		setNpcSkillSeeId(@event => notifySkillSee(@event.getTarget(), @event.getCaster(), @event.getSkill(), @event.getTargets(), @event.isSummon()), npcIds);
	}

	/**
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addSpellFinishedId(params int[] npcIds)
	{
		setNpcSkillFinishedId(@event => notifySpellFinished(@event.getCaster(), @event.getTarget(), @event.getSkill()), npcIds);
	}

	/**
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addSpellFinishedId(IReadOnlyCollection<int> npcIds)
	{
		setNpcSkillFinishedId(@event => notifySpellFinished(@event.getCaster(), @event.getTarget(), @event.getSkill()), npcIds);
	}

	/**
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addTrapActionId(params int[] npcIds)
	{
		setTrapActionId(@event => notifyTrapAction(@event.getTrap(), @event.getTrigger(), @event.getAction()), npcIds);
	}

	/**
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addTrapActionId(IReadOnlyCollection<int> npcIds)
	{
		setTrapActionId(@event => notifyTrapAction(@event.getTrap(), @event.getTrigger(), @event.getAction()), npcIds);
	}

	/**
	 * Add this quest to the list of quests that the passed npc will respond to for faction call events.
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addFactionCallId(params int[] npcIds)
	{
		setAttackableFactionIdId(@event => notifyFactionCall(@event.getNpc(), @event.getCaller(), @event.getAttacker(), @event.isSummon()), npcIds);
	}

	/**
	 * Add this quest to the list of quests that the passed npc will respond to for faction call events.
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addFactionCallId(IReadOnlyCollection<int> npcIds)
	{
		setAttackableFactionIdId(@event => notifyFactionCall(@event.getNpc(), @event.getCaller(), @event.getAttacker(), @event.isSummon()), npcIds);
	}

	/**
	 * Add this quest to the list of quests that the passed npc will respond to for character see events.
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addAggroRangeEnterId(params int[] npcIds)
	{
		setAttackableAggroRangeEnterId(@event => notifyAggroRangeEnter(@event.getNpc(), @event.getActiveChar(), @event.isSummon()), npcIds);
	}

	/**
	 * Add this quest to the list of quests that the passed npc will respond to for character see events.
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addAggroRangeEnterId(IReadOnlyCollection<int> npcIds)
	{
		setAttackableAggroRangeEnterId(@event => notifyAggroRangeEnter(@event.getNpc(), @event.getActiveChar(), @event.isSummon()), npcIds);
	}

	/**
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addCreatureSeeId(params int[] npcIds)
	{
		setCreatureSeeId(@event => notifyCreatureSee((Npc) @event.getCreature(), @event.getSeen()), npcIds);
	}

	/**
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addCreatureSeeId(IReadOnlyCollection<int> npcIds)
	{
		setCreatureSeeId(@event => notifyCreatureSee((Npc) @event.getCreature(), @event.getSeen()), npcIds);
	}

	/**
	 * Register onEnterZone trigger for zone
	 * @param zoneId the ID of the zone to register
	 */
	public void addEnterZoneId(int zoneId)
	{
		setCreatureZoneEnterId(@event => notifyEnterZone(@event.getCreature(), @event.getZone()), zoneId);
	}

	/**
	 * Register onEnterZone trigger for zones
	 * @param zoneIds the IDs of the zones to register
	 */
	public void addEnterZoneId(params int[] zoneIds)
	{
		setCreatureZoneEnterId(@event => notifyEnterZone(@event.getCreature(), @event.getZone()), zoneIds);
	}

	/**
	 * Register onEnterZone trigger for zones
	 * @param zoneIds the IDs of the zones to register
	 */
	public void addEnterZoneId(IReadOnlyCollection<int> zoneIds)
	{
		setCreatureZoneEnterId(@event => notifyEnterZone(@event.getCreature(), @event.getZone()), zoneIds);
	}

	/**
	 * Register onExitZone trigger for zone
	 * @param zoneId the ID of the zone to register
	 */
	public void addExitZoneId(int zoneId)
	{
		setCreatureZoneExitId(@event => notifyExitZone(@event.getCreature(), @event.getZone()), zoneId);
	}

	/**
	 * Register onExitZone trigger for zones
	 * @param zoneIds the IDs of the zones to register
	 */
	public void addExitZoneId(params int[] zoneIds)
	{
		setCreatureZoneExitId(@event => notifyExitZone(@event.getCreature(), @event.getZone()), zoneIds);
	}

	/**
	 * Register onExitZone trigger for zones
	 * @param zoneIds the IDs of the zones to register
	 */
	public void addExitZoneId(IReadOnlyCollection<int> zoneIds)
	{
		setCreatureZoneExitId(@event => notifyExitZone(@event.getCreature(), @event.getZone()), zoneIds);
	}

	/**
	 * Register onEventReceived trigger for NPC
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addEventReceivedId(params int[] npcIds)
	{
		setNpcEventReceivedId(@event => notifyEventReceived(@event.getEventName(), @event.getSender(), @event.getReceiver(), @event.getReference()), npcIds);
	}

	/**
	 * Register onEventReceived trigger for NPC
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addEventReceivedId(IReadOnlyCollection<int> npcIds)
	{
		setNpcEventReceivedId(@event => notifyEventReceived(@event.getEventName(), @event.getSender(), @event.getReceiver(), @event.getReference()), npcIds);
	}

	/**
	 * Register onMoveFinished trigger for NPC
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addMoveFinishedId(params int[] npcIds)
	{
		setNpcMoveFinishedId(@event => notifyMoveFinished(@event.Npc), npcIds);
	}

	/**
	 * Register onMoveFinished trigger for NPC
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addMoveFinishedId(IReadOnlyCollection<int> npcIds)
	{
		setNpcMoveFinishedId(@event => notifyMoveFinished(@event.Npc), npcIds);
	}

	/**
	 * Register onRouteFinished trigger for NPC
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addRouteFinishedId(params int[] npcIds)
	{
		setNpcMoveRouteFinishedId(@event => notifyRouteFinished(@event.getNpc()), npcIds);
	}

	/**
	 * Register onRouteFinished trigger for NPC
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addRouteFinishedId(IReadOnlyCollection<int> npcIds)
	{
		setNpcMoveRouteFinishedId(@event => notifyRouteFinished(@event.getNpc()), npcIds);
	}

	/**
	 * Register onNpcHate trigger for NPC
	 * @param npcIds
	 */
	public void addNpcHateId(params int[] npcIds)
	{
		addNpcHateId(ev =>
		{
			if (!onNpcHate(ev.getNpc(), ev.getActiveChar(), ev.isSummon()))
				ev.Terminate = true;
		}, npcIds);
	}

	/**
	 * Register onNpcHate trigger for NPC
	 * @param npcIds
	 */
	public void addNpcHateId(IReadOnlyCollection<int> npcIds)
	{
		addNpcHateId(ev =>
		{
			if (!onNpcHate(ev.getNpc(), ev.getActiveChar(), ev.isSummon()))
				ev.Terminate = true;
		}, npcIds);
	}

	/**
	 * Register onSummonSpawn trigger when summon is spawned.
	 * @param npcIds
	 */
	public void addSummonSpawnId(params int[] npcIds)
	{
		setPlayerSummonSpawnId(@event => onSummonSpawn(@event.getSummon()), npcIds);
	}

	/**
	 * Register onSummonSpawn trigger when summon is spawned.
	 * @param npcIds
	 */
	public void addSummonSpawnId(IReadOnlyCollection<int> npcIds)
	{
		setPlayerSummonSpawnId(@event => onSummonSpawn(@event.getSummon()), npcIds);
	}

	/**
	 * Register onSummonTalk trigger when master talked to summon.
	 * @param npcIds
	 */
	public void addSummonTalkId(params int[] npcIds)
	{
		setPlayerSummonTalkId(@event => onSummonTalk(@event.getSummon()), npcIds);
	}

	/**
	 * Register onSummonTalk trigger when summon is spawned.
	 * @param npcIds
	 */
	public void addSummonTalkId(IReadOnlyCollection<int> npcIds)
	{
		setPlayerSummonTalkId(@event => onSummonTalk(@event.getSummon()), npcIds);
	}

	/**
	 * Registers onCanSeeMe trigger whenever an npc info must be sent to player.
	 * @param npcIds
	 */
	public void addCanSeeMeId(params int[] npcIds)
	{
		addNpcHateId(ev =>
		{
			if (!notifyOnCanSeeMe(ev.getNpc(), ev.getActiveChar()))
				ev.Terminate = true;
		}, npcIds);
	}

	/**
	 * Registers onCanSeeMe trigger whenever an npc info must be sent to player.
	 * @param npcIds
	 */
	public void addCanSeeMeId(IReadOnlyCollection<int> npcIds)
	{
		addNpcHateId(ev =>
		{
			if (!notifyOnCanSeeMe(ev.getNpc(), ev.getActiveChar()))
				ev.Terminate = true;
		}, npcIds);
	}

	public void addOlympiadMatchFinishId()
	{
		setOlympiadMatchResult(@event => notifyOlympiadMatch(@event.getWinner(), @event.getLoser()));
	}

	/**
	 * Register onInstanceCreated trigger when instance is created.
	 * @param templateIds
	 */
	public void addInstanceCreatedId(params int[] templateIds)
	{
		setInstanceCreatedId(@event => onInstanceCreated(@event.getInstanceWorld(), @event.getCreator()), templateIds);
	}

	/**
	 * Register onInstanceCreated trigger when instance is created.
	 * @param templateIds
	 */
	public void addInstanceCreatedId(IReadOnlyCollection<int> templateIds)
	{
		setInstanceCreatedId(@event => onInstanceCreated(@event.getInstanceWorld(), @event.getCreator()), templateIds);
	}

	/**
	 * Register onInstanceDestroy trigger when instance is destroyed.
	 * @param templateIds
	 */
	public void addInstanceDestroyId(params int[] templateIds)
	{
		setInstanceDestroyId(@event => onInstanceDestroy(@event.getInstanceWorld()), templateIds);
	}

	/**
	 * Register onInstanceCreate trigger when instance is destroyed.
	 * @param templateIds
	 */
	public void addInstanceDestroyId(IReadOnlyCollection<int> templateIds)
	{
		setInstanceDestroyId(@event => onInstanceDestroy(@event.getInstanceWorld()), templateIds);
	}

	/**
	 * Register onInstanceEnter trigger when player enter into instance.
	 * @param templateIds
	 */
	public void addInstanceEnterId(params int[] templateIds)
	{
		setInstanceEnterId(@event => onInstanceEnter(@event.getPlayer(), @event.getInstanceWorld()), templateIds);
	}

	/**
	 * Register onInstanceEnter trigger when player enter into instance.
	 * @param templateIds
	 */
	public void addInstanceEnterId(IReadOnlyCollection<int> templateIds)
	{
		setInstanceEnterId(@event => onInstanceEnter(@event.getPlayer(), @event.getInstanceWorld()), templateIds);
	}

	/**
	 * Register onInstanceEnter trigger when player leave from instance.
	 * @param templateIds
	 */
	public void addInstanceLeaveId(params int[] templateIds)
	{
		setInstanceLeaveId(@event => onInstanceLeave(@event.getPlayer(), @event.getInstanceWorld()), templateIds);
	}

	/**
	 * Register onInstanceEnter trigger when player leave from instance.
	 * @param templateIds
	 */
	public void addInstanceLeaveId(IReadOnlyCollection<int> templateIds)
	{
		setInstanceLeaveId(@event => onInstanceLeave(@event.getPlayer(), @event.getInstanceWorld()), templateIds);
	}

	/**
	 * Use this method to get a random party member from a player's party.<br>
	 * Useful when distributing rewards after killing an NPC.
	 * @param player this parameter represents the player whom the party will taken.
	 * @return {@code null} if {@code player} is {@code null}, {@code player} itself if the player does not have a party, and a random party member in all other cases
	 */
	public Player? getRandomPartyMember(Player player)
	{
		if (player == null)
		{
			return null;
		}
		Party? party = player.getParty();
		if (party == null || party.getMembers().Count == 0)
		{
			return player;
		}
		return party.getMembers().GetRandomElement();
	}

	/**
	 * Get a random party member with required cond value.
	 * @param player the instance of a player whose party is to be searched
	 * @param cond the value of the "cond" variable that must be matched
	 * @return a random party member that matches the specified condition, or {@code null} if no match was found
	 */
	public Player? getRandomPartyMember(Player player, int cond)
	{
		return getRandomPartyMember(player, "cond", cond.ToString(CultureInfo.InvariantCulture));
	}

	/**
	 * Auxiliary function for party quests.<br>
	 * Note: This function is only here because of how commonly it may be used by quest developers.<br>
	 * For any variations on this function, the quest script can always handle things on its own.
	 * @param player the instance of a player whose party is to be searched
	 * @param var the quest variable to look for in party members. If {@code null}, it simply unconditionally returns a random party member
	 * @param value the value of the specified quest variable the random party member must have
	 * @return a random party member that matches the specified conditions or {@code null} if no match was found.<br>
	 *         If the {@code var} parameter is {@code null}, a random party member is selected without any conditions.<br>
	 *         The party member must be within a range of 1500 ingame units of the target of the reference player, or, if no target exists, within the same range of the player itself
	 */
	public Player? getRandomPartyMember(Player player, string var, string value)
	{
		// if no valid player instance is passed, there is nothing to check...
		if (player == null)
		{
			return null;
		}

		// for null var condition, return any random party member.
		if (var == null)
		{
			return getRandomPartyMember(player);
		}

		// normal cases...if the player is not in a party, check the player's state
		QuestState? temp = null;
		Party? party = player.getParty();
		// if this player is not in a party, just check if this player instance matches the conditions itself
		if (party == null || party.getMembers().Count == 0)
		{
			temp = player.getQuestState(Name);
			if (temp != null && temp.isSet(var) && temp.get(var).equalsIgnoreCase(value))
			{
				return player; // match
			}
			return null; // no match
		}

		// if the player is in a party, gather a list of all matching party members (possibly including this player)
		List<Player> candidates = new();
		// get the target for enforcing distance limitations.
		WorldObject? target = player.getTarget();
		if (target == null)
		{
			target = player;
		}

		foreach (Player partyMember in party.getMembers())
		{
			if (partyMember == null)
			{
				continue;
			}
			temp = partyMember.getQuestState(Name);
			if (temp != null && temp.get(var) != null && temp.get(var).equalsIgnoreCase(value) &&
			    partyMember.IsInsideRadius3D(target, Config.ALT_PARTY_RANGE))
			{
				candidates.Add(partyMember);
			}
		}
		// if there was no match, return null...
		if (candidates.Count == 0)
		{
			return null;
		}
		// if a match was found from the party, return one of them at random.
		return candidates.GetRandomElement();
	}

	/**
	 * Auxiliary function for party quests.<br>
	 * Note: This function is only here because of how commonly it may be used by quest developers.<br>
	 * For any variations on this function, the quest script can always handle things on its own.
	 * @param player the player whose random party member is to be selected
	 * @param state the quest state required of the random party member
	 * @return {@code null} if nothing was selected or a random party member that has the specified quest state
	 */
	public Player? getRandomPartyMemberState(Player player, byte state)
	{
		// if no valid player instance is passed, there is nothing to check...
		if (player == null)
		{
			return null;
		}

		// normal cases...if the player is not in a party check the player's state
		QuestState? temp = null;
		Party? party = player.getParty();
		// if this player is not in a party, just check if this player instance matches the conditions itself
		if (party == null || party.getMembers().Count == 0)
		{
			temp = player.getQuestState(Name);
			if (temp != null && temp.getState() == state)
			{
				return player; // match
			}

			return null; // no match
		}

		// if the player is in a party, gather a list of all matching party members (possibly
		// including this player)
		List<Player> candidates = new();

		// get the target for enforcing distance limitations.
		WorldObject? target = player.getTarget();
		if (target == null)
		{
			target = player;
		}

		foreach (Player partyMember in party.getMembers())
		{
			if (partyMember == null)
			{
				continue;
			}
			temp = partyMember.getQuestState(Name);
			if (temp != null && temp.getState() == state && partyMember.IsInsideRadius3D(target, Config.ALT_PARTY_RANGE))
			{
				candidates.Add(partyMember);
			}
		}
		// if there was no match, return null...
		if (candidates.Count == 0)
		{
			return null;
		}

		// if a match was found from the party, return one of them at random.
		return candidates.GetRandomElement();
	}

	/**
	 * Get a random party member from the specified player's party.<br>
	 * If the player is not in a party, only the player himself is checked.<br>
	 * The lucky member is chosen by standard loot roll rules -<br>
	 * each member rolls a random number, the one with the highest roll wins.
	 * @param player the player whose party to check
	 * @param npc the NPC used for distance and other checks (if {@link #checkPartyMember(Player, Npc)} is overriden)
	 * @return the random party member or {@code null}
	 */
	public Player? getRandomPartyMember(Player player, Npc npc)
	{
		if (player == null || !checkDistanceToTarget(player, npc))
		{
			return null;
		}
		Party? party = player.getParty();
		Player? luckyPlayer = null;
		if (party == null)
		{
			if (checkPartyMember(player, npc))
			{
				luckyPlayer = player;
			}
		}
		else
		{
			int highestRoll = 0;
			foreach (Player member in party.getMembers())
			{
				int rnd = getRandom(1000);
				if (rnd > highestRoll && checkPartyMember(member, npc))
				{
					highestRoll = rnd;
					luckyPlayer = member;
				}
			}
		}
		return luckyPlayer != null && checkDistanceToTarget(luckyPlayer, npc) ? luckyPlayer : null;
	}

	/**
	 * This method is called for every party member in {@link #getRandomPartyMember(Player, Npc)}.<br>
	 * It is intended to be overriden by the specific quest implementations.
	 * @param player the player to check
	 * @param npc the NPC that was passed to {@link #getRandomPartyMember(Player, Npc)}
	 * @return {@code true} if this party member passes the check, {@code false} otherwise
	 */
	public bool checkPartyMember(Player player, Npc npc)
	{
		return true;
	}

	/**
	 * Get a random party member from the player's party who has this quest at the specified quest progress.<br>
	 * If the player is not in a party, only the player himself is checked.
	 * @param player the player whose random party member state to get
	 * @param condition the quest progress step the random member should be at (-1 = check only if quest is started)
	 * @param playerChance how many times more chance does the player get compared to other party members (3 - 3x more chance).<br>
	 *            On retail servers, the killer usually gets 2-3x more chance than other party members
	 * @param target the NPC to use for the distance check (can be null)
	 * @return the {@link QuestState} object of the random party member or {@code null} if none matched the condition
	 */
	public QuestState? getRandomPartyMemberState(Player player, QuestCondType condition, int playerChance, Npc target)
	{
		if (player == null || playerChance < 1)
		{
			return null;
		}

		QuestState? qs = player.getQuestState(Name);
        Party? party = player.getParty();
		if (!player.isInParty() || party == null)
		{
			return !checkPartyMemberConditions(qs, condition, target) || !checkDistanceToTarget(player, target) ? null : qs;
		}

		List<QuestState> candidates = new();
		if (qs != null && checkPartyMemberConditions(qs, condition, target) && playerChance > 0)
		{
			for (int i = 0; i < playerChance; i++)
			{
				candidates.Add(qs);
			}
		}

		foreach (Player member in party.getMembers())
		{
			if (member == player)
			{
				continue;
			}

			qs = member.getQuestState(Name);
			if (qs != null && checkPartyMemberConditions(qs, condition, target))
			{
				candidates.Add(qs);
			}
		}

		if (candidates.Count == 0)
		{
			return null;
		}

		qs = candidates.GetRandomElement();
		return !checkDistanceToTarget(qs.getPlayer(), target) ? null : qs;
	}

	private bool checkPartyMemberConditions(QuestState? qs, QuestCondType condition, Npc npc)
	{
		return qs != null && (condition == (QuestCondType)(-1) ? qs.isStarted() : qs.isCond(condition)) && checkPartyMember(qs, npc);
	}

	private static bool checkDistanceToTarget(Player player, Npc target)
	{
		return target == null || Util.checkIfInRange(Config.ALT_PARTY_RANGE, player, target, true);
	}

	/**
	 * This method is called for every party member in {@link #getRandomPartyMemberState(Player, int, int, Npc)} if/after all the standard checks are passed.<br>
	 * It is intended to be overriden by the specific quest implementations.<br>
	 * It can be used in cases when there are more checks performed than simply a quest condition check,<br>
	 * for example, if an item is required in the player's inventory.
	 * @param qs the {@link QuestState} object of the party member
	 * @param npc the NPC that was passed as the last parameter to {@link #getRandomPartyMemberState(Player, int, int, Npc)}
	 * @return {@code true} if this party member passes the check, {@code false} otherwise
	 */
	public bool checkPartyMember(QuestState qs, Npc npc)
	{
		return true;
	}

	/**
	 * @return the registered quest items IDs.
	 */
	public int[] getRegisteredItemIds()
	{
		return _questItemIds;
	}

	/**
	 * Registers all items that have to be destroyed in case player abort the quest or finish it.
	 * @param items
	 */
	public void registerQuestItems(params int[] items)
	{
		foreach (int id in items)
		{
			if (id != 0 && ItemData.getInstance().getTemplate(id) == null)
			{
				LOGGER.Error(GetType().Name + ": Found registerQuestItems for non existing item: " + id + "!");
			}
		}
		_questItemIds = items;
	}

	/**
	 * Remove all quest items associated with this quest from the specified player's inventory.
	 * @param player the player whose quest items to remove
	 */
	public void removeRegisteredQuestItems(Player player)
	{
		takeItemsByIds(player, -1, _questItemIds);
	}


	public override void Unload()
	{
		unload(true);
	}

	/**
	 * @param removeFromList
	 * @return
	 */
	public void unload(bool removeFromList)
	{
		onSave();

		// Cancel all pending timers before reloading.
		// If timers ought to be restarted, the quest can take care of it with its code (example: save global data indicating what timer must be restarted).
		foreach (List<QuestTimer> timers in _questTimers.Values)
		{
			foreach (QuestTimer timer in timers)
			{
				timer.cancel();
			}
			timers.Clear();
		}
		_questTimers.Clear();

		if (removeFromList)
		{
			QuestManager.getInstance().removeScript(this);
			base.Unload();
		}

		base.Unload();
	}

	public void setOnEnterWorld(bool value)
	{
		if (value)
		{
			setPlayerLoginId(@event => notifyEnterWorld(@event.getPlayer()));
		}
		else
		{
			GlobalEvents.Global.UnsubscribeAll<OnPlayerLogin>(this);
		}
	}

	/**
	 * If a quest is set as custom, it will display it's name in the NPC Quest List.<br>
	 * Retail quests are unhardcoded to display the name using a client string.
	 * @param value if {@code true} the quest script will be set as custom quest.
	 */
	public void setCustom(bool value)
	{
		_isCustom = value;
	}

	/**
	 * Verifies if this is a custom quest.
	 * @return {@code true} if the quest script is a custom quest, {@code false} otherwise.
	 */
	public bool isCustomQuest()
	{
		return _isCustom;
	}

	public Set<NpcLogListHolder> getNpcLogList(Player? player)
	{
		return new();
	}

	public bool isTarget<T>(int[] ids, WorldObject target)
		where T: WorldObject
	{
		if (target != null && target is T)
		{
			return Array.IndexOf(ids, target.getId()) >= 0;
		}
		return false;
	}

	public void sendNpcLogList(Player player)
	{
		if (player.getQuestState(Name) != null)
		{
			ExQuestNpcLogListPacket packet = new ExQuestNpcLogListPacket(_questId);
			foreach (NpcLogListHolder holder in getNpcLogList(player))
			{
				packet.add(holder);
			}

			player.sendPacket(packet);
		}
	}

	/**
	 * Gets the start conditions.
	 * @return the start conditions
	 */
	private Set<QuestCondition> getStartConditions()
	{
		return _startCondition;
	}

	/**
	 * Verifies if the player meets all the start conditions.
	 * @param player the player
	 * @return {@code true} if all conditions are met
	 */
	public bool canStartQuest(Player player)
	{
		foreach (QuestCondition cond in _startCondition)
		{
			if (!cond.test(player))
			{
				return false;
			}
		}
		return true;
	}

	/**
	 * Gets the HTML for the first starting condition not met.
	 * @param player the player
	 * @param npc
	 * @return the HTML
	 */
	public string? getStartConditionHtml(Player player, Npc npc)
	{
		QuestState? qs = getQuestState(player, false);
		if (qs != null && !qs.isCreated())
		{
			return null;
		}

		foreach (QuestCondition cond in _startCondition)
		{
			if (!cond.test(player))
			{
				return cond.getHtml(npc);
			}
		}

		return null;
	}

	/**
	 * Adds a predicate to the start conditions.
	 * @param questStartRequirement the predicate condition
	 * @param html the HTML to display if that condition is not met
	 */
	public void addCondStart(Predicate<Player> questStartRequirement, string? html)
	{
		getStartConditions().add(new QuestCondition(questStartRequirement, html));
	}

	/**
	 * Adds a predicate to the start conditions.
	 * @param questStartRequirement the predicate condition
	 * @param pairs the HTML to display if the condition is not met per each npc
	 */
	public void addCondStart(Predicate<Player> questStartRequirement, params KeyValuePair<int, string>[] pairs)
	{
		getStartConditions().add(new QuestCondition(questStartRequirement, pairs));
	}

	/**
	 * Adds a minimum/maximum level start condition to the quest.
	 * @param minLevel the minimum player's level to start the quest
	 * @param maxLevel the maximum player's level to start the quest
	 * @param html the HTML to display if the condition is not met
	 */
	public void addCondLevel(int minLevel, int maxLevel, string html)
	{
		addCondStart(p => p.getLevel() >= minLevel && p.getLevel() <= maxLevel, html);
	}

	/**
	 * Adds a minimum/maximum level start condition to the quest.
	 * @param minLevel the minimum player's level to start the quest
	 * @param maxLevel the maximum player's level to start the quest
	 * @param pairs the HTML to display if the condition is not met per each npc
	 */
	public void addCondMinLevel(int minLevel, int maxLevel, params KeyValuePair<int, string>[] pairs)
	{
		addCondStart(p => p.getLevel() >= minLevel && p.getLevel() <= maxLevel, pairs);
	}

	/**
	 * Adds a minimum level start condition to the quest.
	 * @param minLevel the minimum player's level to start the quest
	 * @param html the HTML to display if the condition is not met
	 */
	public void addCondMinLevel(int minLevel, string? html)
	{
		addCondStart(p => p.getLevel() >= minLevel, html);
	}

	/**
	 * Adds a minimum level start condition to the quest.
	 * @param minLevel the minimum player's level to start the quest
	 * @param pairs the HTML to display if the condition is not met per each npc
	 */
	public void addCondMinLevel(int minLevel, params KeyValuePair<int, string>[] pairs)
	{
		addCondStart(p => p.getLevel() >= minLevel, pairs);
	}

	/**
	 * Adds a minimum/maximum level start condition to the quest.
	 * @param maxLevel the maximum player's level to start the quest
	 * @param html the HTML to display if the condition is not met
	 */
	public void addCondMaxLevel(int maxLevel, string? html)
	{
		addCondStart(p => p.getLevel() <= maxLevel, html);
	}

	/**
	 * Adds a minimum/maximum level start condition to the quest.
	 * @param maxLevel the maximum player's level to start the quest
	 * @param pairs the HTML to display if the condition is not met per each npc
	 */
	public void addCondMaxLevel(int maxLevel, params KeyValuePair<int, string>[] pairs)
	{
		addCondStart(p => p.getLevel() <= maxLevel, pairs);
	}

	/**
	 * Adds a race start condition to the quest.
	 * @param race the race
	 * @param html the HTML to display if the condition is not met
	 */
	public void addCondRace(Race race, string html)
	{
		addCondStart(p => p.getRace() == race, html);
	}

	/**
	 * Adds a race start condition to the quest.
	 * @param race the race
	 * @param pairs the HTML to display if the condition is not met per each npc
	 */
	public void addCondRace(Race race, params KeyValuePair<int, string>[] pairs)
	{
		addCondStart(p => p.getRace() == race, pairs);
	}

	/**
	 * Adds a not-race start condition to the quest.
	 * @param race the race
	 * @param html the HTML to display if the condition is not met
	 */
	public void addCondNotRace(Race race, string html)
	{
		addCondStart(p => p.getRace() != race, html);
	}

	/**
	 * Adds a not-race start condition to the quest.
	 * @param race the race
	 * @param pairs the HTML to display if the condition is not met per each npc
	 */
	public void addCondNotRace(Race race, params KeyValuePair<int, string>[] pairs)
	{
		addCondStart(p => p.getRace() != race, pairs);
	}

	/**
	 * Adds a quest completed start condition to the quest.
	 * @param name the quest name
	 * @param html the HTML to display if the condition is not met
	 */
	public void addCondCompletedQuest(string name, string html)
	{
		addCondStart(p => p.getQuestState(name)?.isCompleted() ?? false, html);
	}

	/**
	 * Adds a quest completed start condition to the quest.
	 * @param name the quest name
	 * @param pairs the HTML to display if the condition is not met per each npc
	 */
	public void addCondCompletedQuest(string name, params KeyValuePair<int, string>[] pairs)
	{
		addCondStart(p => p.getQuestState(name)?.isCompleted() ?? false, pairs);
	}

	/**
	 * Adds a quest started start condition to the quest.
	 * @param name the quest name
	 * @param html the HTML to display if the condition is not met
	 */
	public void addCondStartedQuest(string name, string html)
	{
		addCondStart(p => p.getQuestState(name)?.isStarted() ?? false, html);
	}

	/**
	 * Adds a quest started start condition to the quest.
	 * @param name the quest name
	 * @param pairs the HTML to display if the condition is not met per each npc
	 */
	public void addCondStartedQuest(string name, params KeyValuePair<int, string>[] pairs)
	{
		addCondStart(p => p.getQuestState(name)?.isStarted() ?? false, pairs);
	}

	/**
	 * Adds a class ID start condition to the quest.
	 * @param classId the class ID
	 * @param html the HTML to display if the condition is not met
	 */
	public void addCondClassId(CharacterClass classId, string html)
	{
		addCondStart(p => p.getClassId() == classId, html);
	}

	/**
	 * Adds a class ID start condition to the quest.
	 * @param classId the class ID
	 * @param pairs the HTML to display if the condition is not met per each npc
	 */
	public void addCondClassId(CharacterClass classId, params KeyValuePair<int, string>[] pairs)
	{
		addCondStart(p => p.getClassId() == classId, pairs);
	}

	/**
	 * Adds a class IDs start condition to the quest.
	 * @param classIds the class ID
	 * @param html the HTML to display if the condition is not met
	 */
	public void addCondClassIds(List<CharacterClass> classIds, string html)
	{
		addCondStart(p => classIds.Contains(p.getClassId()), html);
	}

	public void addNewQuestConditions(NewQuestCondition condition, string? html)
	{
		if (condition.getAllowedClassIds().Count != 0)
		{
			addCondStart(p => condition.getAllowedClassIds().Contains(p.getClassId()), html);
		}

		if (condition.getPreviousQuestIds().Count != 0)
		{
			foreach (int questId in condition.getPreviousQuestIds())
			{
				Quest? quest = QuestManager.getInstance().getQuest(questId);
				if (quest != null)
				{
					if (!condition.getOneOfPreQuests())
					{
						addCondStart(p => p.hasQuestState(quest.Name) && p.getQuestState(quest.Name)!.isCompleted(), html);
					}
					else
					{
						addCondStart(p => p.hasAnyCompletedQuestStates(condition.getPreviousQuestIds()), html);
					}
				}
			}

			addCondMinLevel(condition.getMinLevel(), html);
			addCondMaxLevel(condition.getMaxLevel(), html);
		}
	}

	/**
	 * Adds a not-class ID start condition to the quest.
	 * @param classId the class ID
	 * @param html the HTML to display if the condition is not met
	 */
	public void addCondNotClassId(CharacterClass classId, string html)
	{
		addCondStart(p => p.getClassId() != classId, html);
	}

	/**
	 * Adds a not-class ID start condition to the quest.
	 * @param classId the class ID
	 * @param pairs the HTML to display if the condition is not met per each npc
	 */
	public void addCondNotClassId(CharacterClass classId, params KeyValuePair<int, string>[] pairs)
	{
		addCondStart(p => p.getClassId() != classId, pairs);
	}

	/**
	 * Adds a subclass active start condition to the quest.
	 * @param html the HTML to display if the condition is not met
	 */
	public void addCondIsSubClassActive(string html)
	{
		addCondStart(p => p.isSubClassActive(), html);
	}

	/**
	 * Adds a subclass active start condition to the quest.
	 * @param pairs the HTML to display if the condition is not met per each npc
	 */
	public void addCondIsSubClassActive(params KeyValuePair<int, string>[] pairs)
	{
		addCondStart(p => p.isSubClassActive(), pairs);
	}

	/**
	 * Adds a not-subclass active start condition to the quest.
	 * @param html the HTML to display if the condition is not met
	 */
	public void addCondIsNotSubClassActive(string html)
	{
		addCondStart(p => !p.isSubClassActive() && !p.isDualClassActive(), html);
	}

	/**
	 * Adds a not-subclass active start condition to the quest.
	 * @param pairs the HTML to display if the condition is not met per each npc
	 */
	public void addCondIsNotSubClassActive(params KeyValuePair<int, string>[] pairs)
	{
		addCondStart(p => !p.isSubClassActive() && !p.isDualClassActive(), pairs);
	}

	/**
	 * Adds a category start condition to the quest.
	 * @param categoryType the category type
	 * @param html the HTML to display if the condition is not met
	 */
	public void addCondInCategory(CategoryType categoryType, string html)
	{
		addCondStart(p => p.isInCategory(categoryType), html);
	}

	/**
	 * Adds a category start condition to the quest.
	 * @param categoryType the category type
	 * @param pairs the HTML to display if the condition is not met per each npc
	 */
	public void addCondInCategory(CategoryType categoryType, params KeyValuePair<int, string>[] pairs)
	{
		addCondStart(p => p.isInCategory(categoryType), pairs);
	}

	/**
	 * Adds a clan level start condition to the quest.
	 * @param clanLevel the clan level
	 * @param html the HTML to display if the condition is not met
	 */
	public void addCondClanLevel(int clanLevel, string html)
	{
		addCondStart(p => p.getClan()?.getLevel() > clanLevel, html);
	}

	/**
	 * Adds a category start condition to the quest.
	 * @param clanLevel the clan level
	 * @param pairs the HTML to display if the condition is not met per each npc
	 */
	public void addCondClanLevel(int clanLevel, params KeyValuePair<int, string>[] pairs)
	{
		addCondStart(p => p.getClan()?.getLevel() > clanLevel, pairs);
	}

	public void onQuestAborted(Player player)
	{
	}

	public void giveStoryBuffReward(Npc npc, Player player)
	{
		if (Config.ENABLE_STORY_QUEST_BUFF_REWARD)
		{
			foreach (SkillHolder holder in _storyQuestBuffs)
			{
				SkillCaster.triggerCast(npc, player, holder.getSkill());
			}
		}
	}

	public NewQuest? getQuestData()
	{
		return _questData;
	}

	public void rewardPlayer(Player player)
	{
        if (_questData == null)
            return;

		NewQuestReward reward = _questData.getRewards();
		if (reward.getItems() != null)
		{
			if (reward.getItems() != null && reward.getItems().Count != 0)
			{
				foreach (ItemHolder item in reward.getItems())
				{
					giveItems(player, item);
				}
			}

		}
		if (reward.getLevel() > 0)
		{
			long playerExp = player.getExp();
			long targetExp = ExperienceData.getInstance().getExpForLevel(reward.getLevel());
			if (playerExp < targetExp)
			{
				player.addExpAndSp(targetExp - playerExp, 0);
				player.setCurrentHpMp(player.getMaxHp(), player.getMaxMp());
				player.setCurrentCp(player.getMaxCp());
				player.broadcastUserInfo();
			}
		}

		if (reward.getExp() > 0)
		{
			player.getStat().addExp(reward.getExp());

			player.broadcastUserInfo();
		}

		if (reward.getSp() > 0)
		{
			player.getStat().addSp(reward.getSp());
			player.broadcastUserInfo();
		}
	}

	public void teleportToQuestLocation(Player player, Location loc)
	{
		if (player.isDead())
		{
			player.sendPacket(SystemMessageId.DEAD_CHARACTERS_CANNOT_USE_TELEPORTS);
			return;
		}

		// Players should not be able to teleport if in a special location.
		if (player.getMovieHolder() != null || player.isFishing() || player.isInInstance() || player.isOnEvent() || player.isInOlympiadMode() || player.inObserverMode() || player.isInTraingCamp() || player.isInsideZone(ZoneId.TIMED_HUNTING))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_RIGHT_NOW);
			return;
		}

		// Teleport in combat configuration.
		if (!Config.TELEPORT_WHILE_PLAYER_IN_COMBAT && (player.isInCombat() || player.isCastingNow()))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_WHILE_IN_COMBAT);
			return;
		}

		// Karma related configurations.
		if ((!Config.ALT_GAME_KARMA_PLAYER_CAN_TELEPORT || !Config.ALT_GAME_KARMA_PLAYER_CAN_USE_GK) && player.getReputation() < 0)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_RIGHT_NOW);
			return;
		}

		// Cannot escape effect.
		if (player.isAffected(EffectFlag.CANNOT_ESCAPE))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_TELEPORT_RIGHT_NOW);
			return;
		}

		player.abortCast();
		player.stopMove(null);
		player.teleToLocation(loc);
	}
}