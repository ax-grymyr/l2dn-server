using System.Collections.Immutable;
using System.Reflection;
using L2Dn.Events;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Events.Annotations;
using L2Dn.GameServer.Model.Events.Impl.Attackables;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Events.Impl.Instances;
using L2Dn.GameServer.Model.Events.Impl.Items;
using L2Dn.GameServer.Model.Events.Impl.Npcs;
using L2Dn.GameServer.Model.Events.Impl.Olympiads;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Events.Impl.Sieges;
using L2Dn.GameServer.Model.Events.Impl.Summons;
using L2Dn.GameServer.Model.Events.Impl.Traps;
using L2Dn.GameServer.Model.Events.Impl.Zones;
using L2Dn.GameServer.Model.Events.Timers;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Enchant.Attributes;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Spawns;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Events;

public abstract class AbstractScript: IEventTimerEvent<string>, IEventTimerCancel<string>
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(AbstractScript));
	private readonly Set<EventContainer> _eventContainers = new();
	private readonly DateTime _loadTime = DateTime.UtcNow;
	private TimerExecutor<string>? _timerExecutor;

	protected AbstractScript()
	{
		SubscribeToEvents();
	}

	public virtual string Name => GetType().Name;
	public DateTime LoadTime => _loadTime;

	private void SubscribeToEvents()
	{
		MethodInfo[] methods =
			GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		
		foreach (MethodInfo method in methods)
		{
			SubscribeEventAttribute? subscribeEventAttribute = method.GetCustomAttribute<SubscribeEventAttribute>();
			if (subscribeEventAttribute is not null)
			{
				IReadOnlyCollection<EventContainer> containers =
					SubscriptionHelper.Subscribe(subscribeEventAttribute.Type, this, method);
				
				foreach (EventContainer container in containers)
					_eventContainers.Add(container);
			}
		}
	}
	
	/// <summary>
	/// Unsubscribe from all events by this class.
	/// </summary>
	public virtual void Unload()
	{
		foreach (EventContainer container in _eventContainers)
			container.UnsubscribeAllTypes(this);
        
		if (_timerExecutor != null)
			_timerExecutor.cancelAllTimers();
	}

	public virtual void Reload()
	{
		Unload();
		SubscribeToEvents();
	}
    
	public virtual void onTimerEvent(TimerHolder<string> holder)
	{
		onTimerEvent(holder.getEvent(), holder.getParams(), holder.getNpc(), holder.getPlayer());
	}
	
	public virtual void onTimerCancel(TimerHolder<string> holder)
	{
		onTimerCancel(holder.getEvent(), holder.getParams(), holder.getNpc(), holder.getPlayer());
	}
	
	public virtual void onTimerEvent(string @event, StatSet @params, Npc npc, Player player)
	{
		_logger.Warn("[" + GetType().Name + "]: Timer event arrived at non overriden onTimerEvent method event: " +
		            @event + " npc: " + npc + " player: " + player);
	}
	
	public void onTimerCancel(string @event, StatSet @params, Npc npc, Player player)
	{
	}
	
	/**
	 * @return the {@link TimerExecutor} object that manages timers
	 */
	public TimerExecutor<string> getTimers()
	{
		if (_timerExecutor == null)
		{
			lock (this)
			{
				if (_timerExecutor == null)
				{
					_timerExecutor = new TimerExecutor<string>(this, this);
				}
			}
		}
		return _timerExecutor;
	}
	
	public bool hasTimers()
	{
		return _timerExecutor != null;
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------

	public void addTalkId(int npcId)
	{
		setNpcTalkId(ev => ev.Scripts.Add(this), npcId);
	}

	/**
	 * Add this quest to the list of quests that the passed npc will respond to for Talk Events.
	 * @param npcIds the IDs of the NPCs to register
	 */
	public void addTalkId(params int[] npcIds)
	{
		setNpcTalkId(ev => ev.Scripts.Add(this), npcIds);
	}
	
	public void addTalkId(IReadOnlyCollection<int> npcIds)
	{
		setNpcTalkId(ev => ev.Scripts.Add(this), npcIds);
	}
	
	/**
	 * This function is called whenever a player clicks to the "Quest" link of an NPC that is registered for the quest.
	 * @param npc this parameter contains a reference to the exact instance of the NPC that the player is talking with.
	 * @param talker this parameter contains a reference to the exact instance of the player who is talking to the NPC.
	 * @return the text returned by the event (may be {@code null}, a filename or just text)
	 */
	public virtual string onTalk(Npc npc, Player talker)
	{
		return null;
	}
	
	/**
	 * @param npc
	 * @param player
	 */
	public virtual void notifyTalk(Npc npc, Player player)
	{
		string? res = null;
		try
		{
			// TODO: this is just the check that the script subscribed to OnNpcTalk event of Npc
			// the check must be implemented differently, because it is THIS script instance
			// subscribed to Npc OnNpcTalk event and it can store the Npc ids  
			if (npc.Events.HasSubscribers<OnNpcTalk>())
			{
				OnNpcTalk onNpcTalk = new OnNpcTalk(npc, player);
				if (npc.Events.Notify(onNpcTalk) && onNpcTalk.Scripts.Contains(this))
				{
					res = onTalk(npc, player);
				}
			}
		}
		catch (Exception e)
		{
			showError(player, e);
			return;
		}
		
		player.setLastQuestNpcObject(npc.getObjectId());
		showResult(player, res, npc);
	}
	/**
	 * Show an error message to the specified player.
	 * @param player the player to whom to send the error (must be a GM)
	 * @param t the {@link Throwable} to get the message/stacktrace from
	 * @return {@code false}
	 */
	public bool showError(Player player, Exception exception)
	{
		_logger.Warn(Name + " " + exception);
		if (player != null && player.getAccessLevel().isGm())
		{
			string res = "<html><body><title>Script error</title>" + exception + "</body></html>";
			return showResult(player, res);
		}
		return false;
	}
	
	/**
	 * @param player the player to whom to show the result
	 * @param res the message to show to the player
	 * @return {@code false} if the message was sent, {@code true} otherwise
	 * @see #showResult(Player, String, Npc)
	 */
	public bool showResult(Player player, string res)
	{
		return showResult(player, res, null);
	}
	
	/**
	 * Show a message to the specified player.<br>
	 * <u><i>Concept:</i></u><br>
	 * Three cases are managed according to the value of the {@code res} parameter:<br>
	 * <ul>
	 * <li><u>{@code res} ends with ".htm" or ".html":</u> the contents of the specified HTML file are shown in a dialog window</li>
	 * <li><u>{@code res} starts with "&lt;html&gt;":</u> the contents of the parameter are shown in a dialog window</li>
	 * <li><u>all other cases :</u> the text contained in the parameter is shown in chat</li>
	 * </ul>
	 * @param player the player to whom to show the result
	 * @param npc npc to show the result for
	 * @param res the message to show to the player
	 * @return {@code false} if the message was sent, {@code true} otherwise
	 */
	public bool showResult(Player player, string res, Npc npc)
	{
		if (res == null || res.isEmpty() || player == null)
		{
			return true;
		}
		
		if (res.endsWith(".htm") || res.endsWith(".html"))
		{
			showHtmlFile(player, res, npc);
		}
		else if (res.startsWith("<html>"))
		{
			HtmlContent htmlContent = HtmlContent.LoadFromText(res, player);
			if (npc != null)
			{
				htmlContent.Replace("%objectId%", npc.getObjectId().ToString());
			}
			htmlContent.Replace("%playername%", player.getName());

			NpcHtmlMessagePacket npcReply = new NpcHtmlMessagePacket(npc?.getObjectId(), 0, htmlContent);
			player.sendPacket(npcReply);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		}
		else
		{
			player.sendMessage(res);
		}
		return false;
	}
	/**
	 * Send an HTML file to the specified player.
	 * @param player the player to send the HTML to
	 * @param filename the name of the HTML file to show
	 * @return the contents of the HTML file that was sent to the player
	 * @see #showHtmlFile(Player, String, Npc)
	 */
	public string showHtmlFile(Player player, string filename)
	{
		return showHtmlFile(player, filename, null);
	}
	
	/**
	 * Send an HTML file to the specified player.
	 * @param player the player to send the HTML file to
	 * @param filename the name of the HTML file to show
	 * @param npc the NPC that is showing the HTML file
	 * @return the contents of the HTML file that was sent to the player
	 * @see #showHtmlFile(Player, String, Npc)
	 */
	public virtual string showHtmlFile(Player player, string filename, Npc npc)
	{
		// Create handler to file linked to the quest
		string content = getHtm(player, filename);
		
		// Send message to client if message not empty
		if (content != null)
		{
			if (npc != null)
			{
				content = content.Replace("%objectId%", npc.getObjectId().ToString());
			}
			
			HtmlContent htmlContent = HtmlContent.LoadFromText(content, player);
			htmlContent.Replace("%playername%", player.getName());

			NpcHtmlMessagePacket npcReply = new NpcHtmlMessagePacket(npc?.getObjectId(), 0, htmlContent);
			player.sendPacket(npcReply);
			
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		}
		
		return content;
	}
	
	/**
	 * @param player for language prefix.
	 * @param fileName the html file to be get.
	 * @return the HTML file contents
	 */
	public string? getHtm(Player player, string fileName)
	{
		HtmCache hc = HtmCache.getInstance();

		string filePath = $"scripts/quests/{GetType().Name}/{fileName}";
		string? content = hc.getHtm(filePath, player.getLang());
		if (content == null)
		{
			filePath = "scripts/" + getPath() + "/" + fileName;
			content = hc.getHtm(filePath, player.getLang());
			if (content == null)
			{
				filePath = "scripts/quests/" + Name + "/" + fileName;
				content = hc.getHtm(filePath, player.getLang());
			}
		}

		return content;
	}
	
	/**
	 * @return the path of the quest script
	 */
	public string getPath()
	{
		string path = GetType().FullName.Replace('.', '/');
		return path.Substring(0, path.LastIndexOf('/' + GetType().Name)); // TODO
	}
	
	/**
	 * Provides callback operation when Attackable dies from a player.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setAttackableKillId(Action<OnAttackableKill> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides callback operation when Attackable dies from a player.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setAttackableKillId(Action<OnAttackableKill> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when Attackable dies from a player with return type.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void addCreatureKillId(Action<OnCreatureDeath> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when Attackable dies from a player.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setCreatureKillId(Action<OnCreatureDeath> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Attackable} dies from a {@link Player}.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setCreatureKillId(Action<OnCreatureDeath> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when Attackable dies from a player with return type.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void addCreatureAttackedId(Action<OnCreatureAttacked> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when Attackable dies from a player.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setCreatureAttackedId(Action<OnCreatureAttacked> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Attackable} dies from a {@link Player}.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setCreatureAttackedId(Action<OnCreatureAttacked> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link Player} talk to {@link Npc} for first time.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcFirstTalkId(Action<OnNpcFirstTalk> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Player} talk to {@link Npc} for first time.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcFirstTalkId(Action<OnNpcFirstTalk> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link Player} talk to {@link Npc}.
	 * @param npcIds
	 * @return
	 */
	protected void setNpcTalkId(Action<OnNpcTalk> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Player} talk to {@link Npc}.
	 * @param npcIds
	 * @return
	 */
	protected void setNpcTalkId(Action<OnNpcTalk> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when teleport {@link Npc}.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcTeleportId(Action<OnNpcTeleport> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when teleport {@link Npc}.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcTeleportId(Action<OnNpcTeleport> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link Player} talk to {@link Npc} and must receive quest state.
	 * @param npcIds
	 * @return
	 */
	protected void setNpcQuestStartId(Action<OnNpcQuestStart> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Player} talk to {@link Npc} and must receive quest state.
	 * @param npcIds
	 * @return
	 */
	protected void setNpcQuestStartId(Action<OnNpcQuestStart> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when Npc sees skill from a player.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcSkillSeeId(Action<OnNpcSkillSee> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when Npc sees skill from a player.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcSkillSeeId(Action<OnNpcSkillSee> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when Npc casts skill on a player.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcSkillFinishedId(Action<OnNpcSkillFinished> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when Npc casts skill on a player.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcSkillFinishedId(Action<OnNpcSkillFinished> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when Npc is spawned.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcSpawnId(Action<OnNpcSpawn> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when Npc is spawned.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcSpawnId(Action<OnNpcSpawn> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when Npc is despawned.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcDespawnId(Action<OnNpcDespawn> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when Npc is despawned.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcDespawnId(Action<OnNpcDespawn> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link Npc} receives event from another {@link Npc}
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcEventReceivedId(Action<OnNpcEventReceived> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Npc} receives event from another {@link Npc}
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcEventReceivedId(Action<OnNpcEventReceived> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link Npc} finishes to move.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcMoveFinishedId(Action<OnNpcMoveFinished> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Npc} finishes to move.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcMoveFinishedId(Action<OnNpcMoveFinished> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link Npc} finishes to move on its route.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcMoveRouteFinishedId(Action<OnNpcMoveRouteFinished> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Npc} finishes to move on its route.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcMoveRouteFinishedId(Action<OnNpcMoveRouteFinished> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link Npc} is about to hate and start attacking a creature.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcHateId(Action<OnAttackableHate> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Npc} is about to hate and start attacking a creature.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcHateId(Action<OnAttackableHate> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Npc} is about to hate and start attacking a creature.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void addNpcHateId(Action<OnAttackableHate> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Npc} is about to hate and start attacking a creature.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void addNpcHateId(Action<OnAttackableHate> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link Npc} is about to hate and start attacking a creature.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcCanBeSeenId(Action<OnNpcCanBeSeen> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Npc} is about to hate and start attacking a creature.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setNpcCanBeSeenId(Action<OnNpcCanBeSeen> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link Creature} sees another creature.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setCreatureSeeId(Action<OnCreatureSee> callback, params int[] npcIds)
	{
		foreach (int id in npcIds)
		{
			Npc.addCreatureSeeId(id);
		}

		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Creature} sees another creature.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setCreatureSeeId(Action<OnCreatureSee> callback, IReadOnlyCollection<int> npcIds)
	{
		foreach (int id in npcIds)
		{
			Npc.addCreatureSeeId(id);
		}
		
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when Attackable is under attack to other clan mates.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setAttackableFactionIdId(Action<OnAttackableFactionCall> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when Attackable is under attack to other clan mates.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setAttackableFactionIdId(Action<OnAttackableFactionCall> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when Attackable is attacked from a player.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setAttackableAttackId(Action<OnAttackableAttack> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when Attackable is attacked from a player.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setAttackableAttackId(Action<OnAttackableAttack> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link Player} enters in {@link Attackable}'s aggressive range.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setAttackableAggroRangeEnterId(Action<OnAttackableAggroRangeEnter> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Player} enters in {@link Attackable}'s aggressive range.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setAttackableAggroRangeEnterId(Action<OnAttackableAggroRangeEnter> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link Player} learn's a {@link Skill}.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setPlayerSkillLearnId(Action<OnPlayerSkillLearn> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Player} learn's a {@link Skill}.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setPlayerSkillLearnId(Action<OnPlayerSkillLearn> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link Player} summons a servitor or a pet
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setPlayerSummonSpawnId(Action<OnSummonSpawn> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Player} summons a servitor or a pet
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setPlayerSummonSpawnId(Action<OnSummonSpawn> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link Player} talk with a servitor or a pet
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setPlayerSummonTalkId(Action<OnSummonTalk> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Player} talk with a servitor or a pet
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setPlayerSummonTalkId(Action<OnSummonSpawn> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link Player} summons a servitor or a pet
	 * @param callback
	 * @return
	 */
	protected void setPlayerLoginId(Action<OnPlayerLogin> callback)
	{
		SubscribeToEvent(callback, SubscriptionType.Global);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link Player} summons a servitor or a pet
	 * @param callback
	 * @return
	 */
	protected void setPlayerLogoutId(Action<OnPlayerLogout> callback)
	{
		SubscribeToEvent(callback, SubscriptionType.Global);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link org.l2jmobius.gameserver.model.actor.Creature} Enters on a {@link ZoneType}.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setCreatureZoneEnterId(Action<OnZoneEnter> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.ZoneType, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link org.l2jmobius.gameserver.model.actor.Creature} Enters on a {@link ZoneType}.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setCreatureZoneEnterId(Action<OnZoneEnter> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.ZoneType, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link org.l2jmobius.gameserver.model.actor.Creature} Exits on a {@link ZoneType}.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setCreatureZoneExitId(Action<OnZoneExit> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.ZoneType, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link org.l2jmobius.gameserver.model.actor.Creature} Exits on a {@link ZoneType}.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setCreatureZoneExitId(Action<OnZoneExit> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.ZoneType, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link org.l2jmobius.gameserver.model.actor.instance.Trap} acts.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setTrapActionId(Action<OnTrapAction> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link org.l2jmobius.gameserver.model.actor.instance.Trap} acts.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setTrapActionId(Action<OnTrapAction> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.NpcTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link ItemTemplate} receives an event from {@link Player}.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setItemBypassEvenId(Action<OnItemBypassEvent> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.ItemTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link ItemTemplate} receives an event from {@link Player}.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setItemBypassEvenId(Action<OnItemBypassEvent> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.ItemTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when {@link Player} talk to {@link ItemTemplate}.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setItemTalkId(Action<OnItemTalk> callback, params int[] npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.ItemTemplate, npcIds);
	}
	
	/**
	 * Provides instant callback operation when {@link Player} talk to {@link ItemTemplate}.
	 * @param callback
	 * @param npcIds
	 * @return
	 */
	protected void setItemTalkId(Action<OnItemTalk> callback, IReadOnlyCollection<int> npcIds)
	{
		SubscribeToEvent(callback, SubscriptionType.ItemTemplate, npcIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when Olympiad match finishes.
	 * @param callback
	 * @return
	 */
	protected void setOlympiadMatchResult(Action<OnOlympiadMatchResult> callback)
	{
		SubscribeToEvent(callback, SubscriptionType.Olympiad);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when castle siege begins
	 * @param callback
	 * @param castleIds
	 * @return
	 */
	protected void setCastleSiegeStartId(Action<OnCastleSiegeStart> callback, params int[] castleIds)
	{
		SubscribeToEvent(callback, SubscriptionType.Castle, castleIds);
	}
	
	/**
	 * Provides instant callback operation when castle siege begins
	 * @param callback
	 * @param castleIds
	 * @return
	 */
	protected void setCastleSiegeStartId(Action<OnCastleSiegeStart> callback, IReadOnlyCollection<int> castleIds)
	{
		SubscribeToEvent(callback, SubscriptionType.Castle, castleIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when Castle owner has changed during a siege
	 * @param callback
	 * @param castleIds
	 * @return
	 */
	protected void setCastleSiegeOwnerChangeId(Action<OnCastleSiegeOwnerChange> callback, params int[] castleIds)
	{
		SubscribeToEvent(callback, SubscriptionType.Castle, castleIds);
	}
	
	/**
	 * Provides instant callback operation when Castle owner has changed during a siege
	 * @param callback
	 * @param castleIds
	 * @return
	 */
	protected void setCastleSiegeOwnerChangeId(Action<OnCastleSiegeOwnerChange> callback, IReadOnlyCollection<int> castleIds)
	{
		SubscribeToEvent(callback, SubscriptionType.Castle, castleIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when castle siege ends
	 * @param callback
	 * @param castleIds
	 * @return
	 */
	protected void setCastleSiegeFinishId(Action<OnCastleSiegeFinish> callback, params int[] castleIds)
	{
		SubscribeToEvent(callback, SubscriptionType.Castle, castleIds);
	}
	
	/**
	 * Provides instant callback operation when castle siege ends
	 * @param callback
	 * @param castleIds
	 * @return
	 */
	protected void setCastleSiegeFinishId(Action<OnCastleSiegeFinish> callback, IReadOnlyCollection<int> castleIds)
	{
		SubscribeToEvent(callback, SubscriptionType.Castle, castleIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when player's profession has change
	 * @param callback
	 * @return
	 */
	protected void setPlayerProfessionChangeId(Action<OnPlayerProfessionChange> callback)
	{
		SubscribeToEvent(callback, SubscriptionType.Global);
	}
	
	/**
	 * Provides instant callback operation when player's cancel profession
	 * @param callback
	 * @return
	 */
	protected void setPlayerProfessionCancelId(Action<OnPlayerProfessionCancel> callback)
	{
		SubscribeToEvent(callback, SubscriptionType.Global);
	}
	
	/**
	 * Provides instant callback operation when instance world created
	 * @param callback
	 * @param templateIds
	 * @return
	 */
	protected void setInstanceCreatedId(Action<OnInstanceCreated> callback, params int[] templateIds)
	{
		SubscribeToEvent(callback, SubscriptionType.InstanceTemplate, templateIds);
	}
	
	/**
	 * Provides instant callback operation when instance world created
	 * @param callback
	 * @param templateIds
	 * @return
	 */
	protected void setInstanceCreatedId(Action<OnInstanceCreated> callback, IReadOnlyCollection<int> templateIds)
	{
		SubscribeToEvent(callback, SubscriptionType.InstanceTemplate, templateIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when instance world destroyed
	 * @param callback
	 * @param templateIds
	 * @return
	 */
	protected void setInstanceDestroyId(Action<OnInstanceDestroy> callback, params int[] templateIds)
	{
		SubscribeToEvent(callback, SubscriptionType.InstanceTemplate, templateIds);
	}
	
	/**
	 * Provides instant callback operation when instance world destroyed
	 * @param callback
	 * @param templateIds
	 * @return
	 */
	protected void setInstanceDestroyId(Action<OnInstanceDestroy> callback, IReadOnlyCollection<int> templateIds)
	{
		SubscribeToEvent(callback, SubscriptionType.InstanceTemplate, templateIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when player enters into instance world
	 * @param callback
	 * @param templateIds
	 * @return
	 */
	protected void setInstanceEnterId(Action<OnInstanceEnter> callback, params int[] templateIds)
	{
		SubscribeToEvent(callback, SubscriptionType.InstanceTemplate, templateIds);
	}
	
	/**
	 * Provides instant callback operation when player enters into instance world
	 * @param callback
	 * @param templateIds
	 * @return
	 */
	protected void setInstanceEnterId(Action<OnInstanceEnter> callback, IReadOnlyCollection<int> templateIds)
	{
		SubscribeToEvent(callback, SubscriptionType.InstanceTemplate, templateIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation when player leave from instance world
	 * @param callback
	 * @param templateIds
	 * @return
	 */
	protected void setInstanceLeaveId(Action<OnInstanceLeave> callback, params int[] templateIds)
	{
		SubscribeToEvent(callback, SubscriptionType.InstanceTemplate, templateIds);
	}
	
	/**
	 * Provides instant callback operation when player leave from instance world
	 * @param callback
	 * @param templateIds
	 * @return
	 */
	protected void setInstanceLeaveId(Action<OnInstanceLeave> callback, IReadOnlyCollection<int> templateIds)
	{
		SubscribeToEvent(callback, SubscriptionType.InstanceTemplate, templateIds);
	}
	
	// ---------------------------------------------------------------------------------------------------------------------------
	
	/**
	 * Provides instant callback operation on instance status change
	 * @param callback
	 * @param templateIds
	 * @return
	 */
	protected void setInstanceStatusChangeId(Action<OnInstanceStatusChange> callback, params int[] templateIds)
	{
		SubscribeToEvent(callback, SubscriptionType.InstanceTemplate, templateIds);
	}
	
	/**
	 * Provides instant callback operation on instance status change
	 * @param callback
	 * @param templateIds
	 * @return
	 */
	protected void setInstanceStatusChangeId(Action<OnInstanceStatusChange> callback, IReadOnlyCollection<int> templateIds)
	{
		SubscribeToEvent(callback, SubscriptionType.InstanceTemplate, templateIds);
	}

	// --------------------------------------------------------------------------------------------------
	// --------------------------------------Register methods--------------------------------------------
	// --------------------------------------------------------------------------------------------------
	
	/**
	 * Generic listener register method
	 * @param action
	 * @param registerType
	 * @param ids
	 * @return
	 */
	protected void SubscribeToEvent<TEvent>(Action<TEvent> action, SubscriptionType subscriptionType, params int[] ids)
		where TEvent: EventBase
	{
		SubscribeToEvent(action, subscriptionType, (IReadOnlyCollection<int>)ids);
	}

	/**
	 * Generic listener register method
	 * @param action
	 * @param registerType
	 * @param ids
	 * @return
	 */
	protected void SubscribeToEvent<TEvent>(Action<TEvent> action, SubscriptionType subscriptionType,
		IReadOnlyCollection<int> ids)
		where TEvent: EventBase
	{
		string typeName = "event " + typeof(TEvent).Name + ", class " + (GetType().FullName ?? nameof(AbstractScript));
		ImmutableList<EventContainer> containers =
			SubscriptionHelper.GetEventContainers(subscriptionType, typeName, ids);
		
		foreach (EventContainer container in containers)
		{
			container.Subscribe(this, action);
			_eventContainers.Add(container);
		}
	}
	
	/**
	 * -------------------------------------------------------------------------------------------------------
	 */
	
	/**
	 * @param template
	 */
	public void onSpawnActivate(SpawnTemplate template)
	{
	}
	
	/**
	 * @param template
	 */
	public void onSpawnDeactivate(SpawnTemplate template)
	{
	}
	
	/**
	 * @param template
	 * @param group
	 * @param npc
	 */
	public void onSpawnNpc(SpawnTemplate template, SpawnGroup group, Npc npc)
	{
	}
	
	/**
	 * @param template
	 * @param group
	 * @param npc
	 */
	public void onSpawnDespawnNpc(SpawnTemplate template, SpawnGroup group, Npc npc)
	{
	}
	
	/**
	 * @param template
	 * @param group
	 * @param npc
	 * @param killer
	 */
	public void onSpawnNpcDeath(SpawnTemplate template, SpawnGroup group, Npc npc, Creature killer)
	{
	}
	
	/**
	 * -------------------------------------------------------------------------------------------------------
	 */
	
	/**
	 * Show an on screen message to the player.
	 * @param player the player to display the message to
	 * @param text the message to display
	 * @param time the duration of the message in milliseconds
	 */
	public static void showOnScreenMsg(Player player, string text, int time)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		
		player.sendPacket(new ExShowScreenMessagePacket(text, time));
	}
	
	/**
	 * Show an on screen message to the player.
	 * @param player the player to display the message to
	 * @param npcString the NPC string to display
	 * @param position the position of the message on the screen
	 * @param time the duration of the message in milliseconds
	 * @param params values of parameters to replace in the NPC String (like S1, C1 etc.)
	 */
	public static void showOnScreenMsg(Player player, NpcStringId npcString, int position, int time, params string[] @params)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		
		player.sendPacket(new ExShowScreenMessagePacket(npcString, position, time, @params));
	}
	
	/**
	 * Show an on screen message to the player.
	 * @param player the player to display the message to
	 * @param npcString the NPC string to display
	 * @param position the position of the message on the screen
	 * @param time the duration of the message in milliseconds
	 * @param showEffect the upper effect
	 * @param params values of parameters to replace in the NPC String (like S1, C1 etc.)
	 */
	public static void showOnScreenMsg(Player player, NpcStringId npcString, int position, int time, bool showEffect, params string[] @params)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		player.sendPacket(new ExShowScreenMessagePacket(npcString, position, time, showEffect, @params));
	}
	
	/**
	 * Show an on screen message to the player.
	 * @param player the player to display the message to
	 * @param systemMsg the system message to display
	 * @param position the position of the message on the screen
	 * @param time the duration of the message in milliseconds
	 * @param params values of parameters to replace in the system message (like S1, C1 etc.)
	 */
	public static void showOnScreenMsg(Player player, SystemMessageId systemMsg, int position, int time, params string[] @params)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		player.sendPacket(new ExShowScreenMessagePacket(systemMsg, position, time, @params));
	}
	
	/**
	 * Add a temporary spawn of the specified NPC.
	 * @param npcId the ID of the NPC to spawn
	 * @param pos the object containing the spawn location coordinates
	 * @return the {@link Npc} object of the newly spawned NPC or {@code null} if the NPC doesn't exist
	 * @see #addSpawn(int, IPositionable, bool, long, bool, int)
	 * @see #addSpawn(int, int, int, int, int, bool, long, bool, int)
	 */
	public static Npc addSpawn(int npcId, Location location)
	{
		return addSpawn(npcId, location.X, location.Y, location.Z, location.Heading, false, TimeSpan.Zero, false, 0);
	}
	
	/**
	 * Add a temporary spawn of the specified NPC.
	 * @param summoner the NPC that requires this spawn
	 * @param npcId the ID of the NPC to spawn
	 * @param pos the object containing the spawn location coordinates
	 * @param randomOffset if {@code true}, adds +/- 50~100 to X/Y coordinates of the spawn location
	 * @param despawnDelay time in milliseconds till the NPC is despawned (0 - only despawned on server shutdown)
	 * @return the {@link Npc} object of the newly spawned NPC, {@code null} if the NPC doesn't exist
	 */
	public static Npc addSpawn(Npc summoner, int npcId, Location3D location, int heading, bool randomOffset, TimeSpan despawnDelay)
	{
		return addSpawn(summoner, npcId, location.X, location.Y, location.Z, heading, randomOffset, despawnDelay, false, 0);
	}
	
	/**
	 * Add a temporary spawn of the specified NPC.
	 * @param npcId the ID of the NPC to spawn
	 * @param pos the object containing the spawn location coordinates
	 * @param isSummonSpawn if {@code true}, displays a summon animation on NPC spawn
	 * @return the {@link Npc} object of the newly spawned NPC or {@code null} if the NPC doesn't exist
	 * @see #addSpawn(int, IPositionable, bool, long, bool, int)
	 * @see #addSpawn(int, int, int, int, int, bool, long, bool, int)
	 */
	public static Npc addSpawn(int npcId, Location location, bool isSummonSpawn)
	{
		return addSpawn(npcId, location.X, location.Y, location.Z, location.Heading, false, TimeSpan.Zero, isSummonSpawn, 0);
	}
	
	/**
	 * Add a temporary spawn of the specified NPC.
	 * @param npcId the ID of the NPC to spawn
	 * @param pos the object containing the spawn location coordinates
	 * @param randomOffset if {@code true}, adds +/- 50~100 to X/Y coordinates of the spawn location
	 * @param despawnDelay time in milliseconds till the NPC is despawned (0 - only despawned on server shutdown)
	 * @return the {@link Npc} object of the newly spawned NPC or {@code null} if the NPC doesn't exist
	 * @see #addSpawn(int, IPositionable, bool, long, bool, int)
	 * @see #addSpawn(int, int, int, int, int, bool, long, bool, int)
	 */
	public static Npc addSpawn(int npcId, Location location, bool randomOffset, TimeSpan despawnDelay)
	{
		return addSpawn(npcId, location.X, location.Y, location.Z, location.Heading, randomOffset, despawnDelay, false, 0);
	}
	
	/**
	 * Add a temporary spawn of the specified NPC.
	 * @param npcId the ID of the NPC to spawn
	 * @param pos the object containing the spawn location coordinates
	 * @param randomOffset if {@code true}, adds +/- 50~100 to X/Y coordinates of the spawn location
	 * @param despawnDelay time in milliseconds till the NPC is despawned (0 - only despawned on server shutdown)
	 * @param isSummonSpawn if {@code true}, displays a summon animation on NPC spawn
	 * @return the {@link Npc} object of the newly spawned NPC or {@code null} if the NPC doesn't exist
	 * @see #addSpawn(int, IPositionable, bool, long, bool, int)
	 * @see #addSpawn(int, int, int, int, int, bool, long, bool, int)
	 */
	public static Npc addSpawn(int npcId, Location location, bool randomOffset, TimeSpan despawnDelay, bool isSummonSpawn)
	{
		return addSpawn(npcId, location.X, location.Y, location.Z, location.Heading, randomOffset, despawnDelay, isSummonSpawn, 0);
	}
	
	/**
	 * Add a temporary spawn of the specified NPC.
	 * @param summoner the NPC that requires this spawn
	 * @param npcId the ID of the NPC to spawn
	 * @param pos the object containing the spawn location coordinates
	 * @param randomOffset if {@code true}, adds +/- 50~100 to X/Y coordinates of the spawn location
	 * @param instanceId the ID of the instance to spawn the NPC in (0 - the open world)
	 * @return the {@link Npc} object of the newly spawned NPC or {@code null} if the NPC doesn't exist
	 * @see #addSpawn(int, IPositionable)
	 * @see #addSpawn(int, IPositionable, bool)
	 * @see #addSpawn(int, IPositionable, bool, long)
	 * @see #addSpawn(int, IPositionable, bool, long, bool)
	 * @see #addSpawn(int, int, int, int, int, bool, long, bool, int)
	 */
	public static Npc addSpawn(Npc summoner, int npcId, Location location, bool randomOffset, int instanceId)
	{
		return addSpawn(summoner, npcId, location.X, location.Y, location.Z, location.Heading, randomOffset, TimeSpan.Zero, false, instanceId);
	}
	
	/**
	 * Add a temporary spawn of the specified NPC.
	 * @param npcId the ID of the NPC to spawn
	 * @param pos the object containing the spawn location coordinates
	 * @param randomOffset if {@code true}, adds +/- 50~100 to X/Y coordinates of the spawn location
	 * @param despawnDelay time in milliseconds till the NPC is despawned (0 - only despawned on server shutdown)
	 * @param isSummonSpawn if {@code true}, displays a summon animation on NPC spawn
	 * @param instanceId the ID of the instance to spawn the NPC in (0 - the open world)
	 * @return the {@link Npc} object of the newly spawned NPC or {@code null} if the NPC doesn't exist
	 * @see #addSpawn(int, IPositionable)
	 * @see #addSpawn(int, IPositionable, bool)
	 * @see #addSpawn(int, IPositionable, bool, long)
	 * @see #addSpawn(int, IPositionable, bool, long, bool)
	 * @see #addSpawn(int, int, int, int, int, bool, long, bool, int)
	 */
	public static Npc addSpawn(int npcId, Location location, bool randomOffset, TimeSpan despawnDelay, bool isSummonSpawn, int instanceId)
	{
		return addSpawn(npcId, location.X, location.Y, location.Z, location.Heading, randomOffset, despawnDelay, isSummonSpawn, instanceId);
	}
	
	/**
	 * Add a temporary spawn of the specified NPC.
	 * @param npcId the ID of the NPC to spawn
	 * @param x the X coordinate of the spawn location
	 * @param y the Y coordinate of the spawn location
	 * @param z the Z coordinate (height) of the spawn location
	 * @param heading the heading of the NPC
	 * @param randomOffset if {@code true}, adds +/- 50~100 to X/Y coordinates of the spawn location
	 * @param despawnDelay time in milliseconds till the NPC is despawned (0 - only despawned on server shutdown)
	 * @return the {@link Npc} object of the newly spawned NPC or {@code null} if the NPC doesn't exist
	 * @see #addSpawn(int, IPositionable, bool, long, bool, int)
	 * @see #addSpawn(int, int, int, int, int, bool, long, bool, int)
	 */
	public static Npc addSpawn(int npcId, int x, int y, int z, int heading, bool randomOffset, TimeSpan despawnDelay)
	{
		return addSpawn(npcId, x, y, z, heading, randomOffset, despawnDelay, false, 0);
	}
	
	/**
	 * Add a temporary spawn of the specified NPC.
	 * @param npcId the ID of the NPC to spawn
	 * @param x the X coordinate of the spawn location
	 * @param y the Y coordinate of the spawn location
	 * @param z the Z coordinate (height) of the spawn location
	 * @param heading the heading of the NPC
	 * @param randomOffset if {@code true}, adds +/- 50~100 to X/Y coordinates of the spawn location
	 * @param despawnDelay time in milliseconds till the NPC is despawned (0 - only despawned on server shutdown)
	 * @param isSummonSpawn if {@code true}, displays a summon animation on NPC spawn
	 * @return the {@link Npc} object of the newly spawned NPC or {@code null} if the NPC doesn't exist
	 * @see #addSpawn(int, IPositionable, bool, long, bool, int)
	 * @see #addSpawn(int, int, int, int, int, bool, long, bool, int)
	 */
	public static Npc addSpawn(int npcId, int x, int y, int z, int heading, bool randomOffset, TimeSpan despawnDelay, bool isSummonSpawn)
	{
		return addSpawn(npcId, x, y, z, heading, randomOffset, despawnDelay, isSummonSpawn, 0);
	}
	
	/**
	 * Add a temporary spawn of the specified NPC.
	 * @param npcId the ID of the NPC to spawn
	 * @param x the X coordinate of the spawn location
	 * @param y the Y coordinate of the spawn location
	 * @param z the Z coordinate (height) of the spawn location
	 * @param heading the heading of the NPC
	 * @param randomOffset if {@code true}, adds +/- 50~100 to X/Y coordinates of the spawn location
	 * @param despawnDelay time in milliseconds till the NPC is despawned (0 - only despawned on server shutdown)
	 * @param isSummonSpawn if {@code true}, displays a summon animation on NPC spawn
	 * @param instanceId the ID of the instance to spawn the NPC in (0 - the open world)
	 * @return the {@link Npc} object of the newly spawned NPC or {@code null} if the NPC doesn't exist
	 * @see #addSpawn(int, IPositionable, bool, long, bool, int)
	 * @see #addSpawn(int, int, int, int, int, bool, long)
	 * @see #addSpawn(int, int, int, int, int, bool, long, bool)
	 */
	public static Npc addSpawn(int npcId, int x, int y, int z, int heading, bool randomOffset, TimeSpan despawnDelay, bool isSummonSpawn, int instanceId)
	{
		return addSpawn(null, npcId, x, y, z, heading, randomOffset, despawnDelay, isSummonSpawn, instanceId);
	}
	
	/**
	 * Add a temporary spawn of the specified NPC.
	 * @param summoner the NPC that requires this spawn
	 * @param npcId the ID of the NPC to spawn
	 * @param xValue the X coordinate of the spawn location
	 * @param yValue the Y coordinate of the spawn location
	 * @param zValue the Z coordinate (height) of the spawn location
	 * @param heading the heading of the NPC
	 * @param randomOffset if {@code true}, adds +/- 50~100 to X/Y coordinates of the spawn location
	 * @param despawnDelay time in milliseconds till the NPC is despawned (0 - only despawned on server shutdown)
	 * @param isSummonSpawn if {@code true}, displays a summon animation on NPC spawn
	 * @param instance instance where NPC should be spawned ({@code null} - normal world)
	 * @return the {@link Npc} object of the newly spawned NPC or {@code null} if the NPC doesn't exist
	 * @see #addSpawn(int, IPositionable, bool, long, bool, int)
	 * @see #addSpawn(int, int, int, int, int, bool, long)
	 * @see #addSpawn(int, int, int, int, int, bool, long, bool)
	 */
	public static Npc addSpawn(Npc summoner, int npcId, int xValue, int yValue, int zValue, int heading, bool randomOffset, TimeSpan despawnDelay, bool isSummonSpawn, int instance)
	{
		try
		{
			if ((xValue == 0) && (yValue == 0))
			{
				_logger.Error("addSpawn(): invalid spawn coordinates for NPC #" + npcId + "!");
				return null;
			}
			
			int x = xValue;
			int y = yValue;
			if (randomOffset)
			{
				int offset = Rnd.get(50, 100);
				if (Rnd.nextBoolean())
				{
					offset *= -1;
				}
				x += offset;
				offset = Rnd.get(50, 100);
				if (Rnd.nextBoolean())
				{
					offset *= -1;
				}
				y += offset;
			}
			
			Spawn spawn = new Spawn(npcId);
			spawn.setInstanceId(instance);
			spawn.Location = new Location(x, y, zValue, heading);
			spawn.stopRespawn();
			
			Npc npc = spawn.doSpawn(isSummonSpawn);
			if (despawnDelay > TimeSpan.Zero)
			{
				npc.scheduleDespawn(despawnDelay);
			}
			
			if (summoner != null)
			{
				summoner.addSummonedNpc(npc);
			}
			
			// Retain monster original position if ENABLE_RANDOM_MONSTER_SPAWNS is enabled.
			if (Config.ENABLE_RANDOM_MONSTER_SPAWNS && !randomOffset && npc.isMonster())
			{
				spawn.Location = new Location(x, y, zValue, spawn.Location.Heading);
				npc.setXYZ(x, y, zValue);
				if (heading > -1)
				{
					npc.setHeading(heading);
				}
			}
			
			// Fixes invisible NPCs spawned by script.
			npc.broadcastInfo();
			
			return npc;
		}
		catch (Exception e)
		{
			_logger.Warn("Could not spawn NPC #" + npcId + "; error: " + e);
		}
		
		return null;
	}
	
	/**
	 * @param trapId
	 * @param x
	 * @param y
	 * @param z
	 * @param heading
	 * @param instanceId
	 * @return
	 */
	public Trap addTrap(int trapId, int x, int y, int z, int heading, int instanceId)
	{
		NpcTemplate npcTemplate = NpcData.getInstance().getTemplate(trapId);
		Trap trap = new Trap(npcTemplate, instanceId, -1);
		trap.setCurrentHp(trap.getMaxHp());
		trap.setCurrentMp(trap.getMaxMp());
		trap.setInvul(true);
		trap.setHeading(heading);
		trap.spawnMe(x, y, z);
		return trap;
	}
	
	/**
	 * @param master
	 * @param minionId
	 * @return
	 */
	public Npc addMinion(Monster master, int minionId)
	{
		return MinionList.spawnMinion(master, minionId);
	}
	
	/**
	 * Get the amount of an item in player's inventory.
	 * @param player the player whose inventory to check
	 * @param itemId the ID of the item whose amount to get
	 * @return the amount of the specified item in player's inventory
	 */
	public static long getQuestItemsCount(Player player, int itemId)
	{
		return player.getInventory().getInventoryItemCount(itemId, -1);
	}
	
	/**
	 * Get the total amount of all specified items in player's inventory.
	 * @param player the player whose inventory to check
	 * @param itemIds a list of IDs of items whose amount to get
	 * @return the summary amount of all listed items in player's inventory
	 */
	public long getQuestItemsCount(Player player, params int[] itemIds)
	{
		long count = 0;
		foreach (Item item in player.getInventory().getItems())
		{
			if (item == null)
			{
				continue;
			}
			
			foreach (int itemId in itemIds)
			{
				if (item.getId() == itemId)
				{
					if ((count + item.getCount()) > long.MaxValue) // TODO! overflow
					{
						return long.MaxValue;
					}
					count += item.getCount();
				}
			}
		}
		return count;
	}
	
	/**
	 * Check if the player has the specified item in his inventory.
	 * @param player the player whose inventory to check for the specified item
	 * @param item the {@link ItemHolder} object containing the ID and count of the item to check
	 * @return {@code true} if the player has the required count of the item
	 */
	protected static bool hasItem(Player player, ItemHolder item)
	{
		return hasItem(player, item, true);
	}
	
	/**
	 * Check if the player has the required count of the specified item in his inventory.
	 * @param player the player whose inventory to check for the specified item
	 * @param item the {@link ItemHolder} object containing the ID and count of the item to check
	 * @param checkCount if {@code true}, check if each item is at least of the count specified in the ItemHolder,<br>
	 *            otherwise check only if the player has the item at all
	 * @return {@code true} if the player has the item
	 */
	protected static bool hasItem(Player player, ItemHolder item, bool checkCount)
	{
		if (item == null)
		{
			return false;
		}
		if (checkCount)
		{
			return getQuestItemsCount(player, item.getId()) >= item.getCount();
		}
		return hasQuestItems(player, item.getId());
	}
	
	/**
	 * Check if the player has all the specified items in his inventory and, if necessary, if their count is also as required.
	 * @param player the player whose inventory to check for the specified item
	 * @param checkCount if {@code true}, check if each item is at least of the count specified in the ItemHolder,<br>
	 *            otherwise check only if the player has the item at all
	 * @param itemList a list of {@link ItemHolder} objects containing the IDs of the items to check
	 * @return {@code true} if the player has all the items from the list
	 */
	protected static bool hasAllItems(Player player, bool checkCount, params ItemHolder[] itemList)
	{
		if ((itemList == null) || (itemList.Length == 0))
		{
			return false;
		}
		foreach (ItemHolder item in itemList)
		{
			if (!hasItem(player, item, checkCount))
			{
				return false;
			}
		}
		return true;
	}
	
	/**
	 * Check for an item in player's inventory.
	 * @param player the player whose inventory to check for quest items
	 * @param itemId the ID of the item to check for
	 * @return {@code true} if the item exists in player's inventory, {@code false} otherwise
	 */
	public static bool hasQuestItems(Player player, int itemId)
	{
		return player.getInventory().getItemByItemId(itemId) != null;
	}
	
	/**
	 * Check for multiple items in player's inventory.
	 * @param player the player whose inventory to check for quest items
	 * @param itemIds a list of item IDs to check for
	 * @return {@code true} if all items exist in player's inventory, {@code false} otherwise
	 */
	public static bool hasQuestItems(Player player, params int[] itemIds)
	{
		if ((itemIds == null) || (itemIds.Length == 0))
		{
			return false;
		}
		
		PlayerInventory inv = player.getInventory();
		foreach (int itemId in itemIds)
		{
			if (inv.getItemByItemId(itemId) == null)
			{
				return false;
			}
		}
		return true;
	}
	
	/**
	 * Check for multiple items in player's inventory.
	 * @param player the player whose inventory to check for quest items
	 * @param itemIds a list of item IDs to check for
	 * @return {@code true} if at least one items exist in player's inventory, {@code false} otherwise
	 */
	public bool hasAtLeastOneQuestItem(Player player, params int[] itemIds)
	{
		PlayerInventory inv = player.getInventory();
		foreach (int itemId in itemIds)
		{
			if (inv.getItemByItemId(itemId) != null)
			{
				return true;
			}
		}
		return false;
	}
	
	/**
	 * Extensive player ownership check for single or multiple items.<br>
	 * Checks inventory, warehouse, pet, summons, mail attachments and item auctions.
	 * @param player the player to check for quest items
	 * @param itemIds a list of item IDs to check for
	 * @return {@code true} if player owns at least one items, {@code false} otherwise.
	 */
	public bool ownsAtLeastOneItem(Player player, params int[] itemIds)
	{
		// Inventory.
		PlayerInventory inventory = player.getInventory();
		foreach (int itemId in itemIds)
		{
			if (inventory.getItemByItemId(itemId) != null)
			{
				return true;
			}
		}
		// Warehouse.
		PlayerWarehouse warehouse = player.getWarehouse();
		foreach (int itemId in itemIds)
		{
			if (warehouse.getItemByItemId(itemId) != null)
			{
				return true;
			}
		}
		// Pet.
		if (player.hasPet())
		{
			PetInventory petInventory = player.getPet().getInventory();
			if (petInventory != null)
			{
				foreach (int itemId in itemIds)
				{
					if (petInventory.getItemByItemId(itemId) != null)
					{
						return true;
					}
				}
			}
		}
		// Summons.
		if (player.hasServitors())
		{
			foreach (Summon summon in player.getServitors().values())
			{
				PetInventory summonInventory = summon.getInventory();
				if (summonInventory != null)
				{
					foreach (int itemId in itemIds)
					{
						if (summonInventory.getItemByItemId(itemId) != null)
						{
							return true;
						}
					}
				}
			}
		}
		// Mail attachments.
		if (Config.ALLOW_MAIL)
		{
			List<Message> inbox = MailManager.getInstance().getInbox(player.getObjectId());
			foreach (int itemId in itemIds)
			{
				foreach (Message message in inbox)
				{
					Mail mail = message.getAttachments();
					if ((mail != null) && (mail.getItemByItemId(itemId) != null))
					{
						return true;
					}
				}
			}
			List<Message> outbox = MailManager.getInstance().getOutbox(player.getObjectId());
			foreach (int itemId in itemIds)
			{
				foreach (Message message in outbox)
				{
					Mail mail = message.getAttachments();
					if ((mail != null) && (mail.getItemByItemId(itemId) != null))
					{
						return true;
					}
				}
			}
		}
		// Item auctions.
		foreach (int itemId in itemIds)
		{
			if (ItemCommissionManager.getInstance().hasCommissionedItemId(player, itemId))
			{
				return true;
			}
		}
		return false;
	}
	
	/**
	 * Get the enchantment level of an item in player's inventory.
	 * @param player the player whose item to check
	 * @param itemId the ID of the item whose enchantment level to get
	 * @return the enchantment level of the item or 0 if the item was not found
	 */
	public static int getEnchantLevel(Player player, int itemId)
	{
		Item enchantedItem = player.getInventory().getItemByItemId(itemId);
		if (enchantedItem == null)
		{
			return 0;
		}
		return enchantedItem.getEnchantLevel();
	}
	
	/**
	 * Give Adena to the player.
	 * @param player the player to whom to give the Adena
	 * @param count the amount of Adena to give
	 * @param applyRates if {@code true} quest rates will be applied to the amount
	 */
	public void giveAdena(Player player, long count, bool applyRates)
	{
		if (applyRates)
		{
			rewardItems(player, Inventory.ADENA_ID, count);
		}
		else
		{
			giveItems(player, Inventory.ADENA_ID, count);
		}
	}
	
	/**
	 * Give a reward to player using multipliers.
	 * @param player the player to whom to give the item
	 * @param holder
	 */
	public static void rewardItems(Player player, ItemHolder holder)
	{
		rewardItems(player, holder.getId(), holder.getCount());
	}
	
	/**
	 * Give a reward to player using multipliers.
	 * @param player the player to whom to give the item
	 * @param itemId the ID of the item to give
	 * @param countValue the amount of items to give
	 */
	public static void rewardItems(Player player, int itemId, long countValue)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		
		if (countValue <= 0)
		{
			return;
		}
		
		ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
		if (item == null)
		{
			return;
		}
		
		long count = countValue;
		try
		{
			if (itemId == Inventory.ADENA_ID)
			{
				count = (long)(count * Config.RATE_QUEST_REWARD_ADENA);
			}
			else if (Config.RATE_QUEST_REWARD_USE_MULTIPLIERS)
			{
				if (item is EtcItem etcItem)
				{
					switch (etcItem.getItemType().AsEtcItemType())
					{
						case EtcItemType.POTION:
						{
							count = (long)(count * Config.RATE_QUEST_REWARD_POTION);
							break;
						}
						case EtcItemType.ENCHT_WP:
						case EtcItemType.ENCHT_AM:
						case EtcItemType.SCROLL:
						{
							count = (long)(count * Config.RATE_QUEST_REWARD_SCROLL);
							break;
						}
						case EtcItemType.RECIPE:
						{
							count = (long)(count * Config.RATE_QUEST_REWARD_RECIPE);
							break;
						}
						case EtcItemType.MATERIAL:
						{
							count = (long)(count * Config.RATE_QUEST_REWARD_MATERIAL);
							break;
						}
						default:
						{
							count = (long)(count * Config.RATE_QUEST_REWARD);
							break;
						}
					}
				}
			}
			else
			{
				count = (long)(count * Config.RATE_QUEST_REWARD);
			}
		}
		catch (Exception e)
		{
			count = long.MaxValue;
		}
		
		// Add items to player's inventory
		Item itemInstance = player.getInventory().addItem("Quest", itemId, count, player, player.getTarget());
		if (itemInstance == null)
		{
			return;
		}
		
		sendItemGetMessage(player, itemInstance, count);
	}
	
	/**
	 * Send the system message and the status update packets to the player.
	 * @param player the player that has got the item
	 * @param item the item obtain by the player
	 * @param count the item count
	 */
	private static void sendItemGetMessage(Player player, Item item, long count)
	{
		// If item for reward is gold, send message of gold reward to client
		if (item.getId() == Inventory.ADENA_ID)
		{
			SystemMessagePacket smsg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1_ADENA_2);
			smsg.Params.addLong(count);
			player.sendPacket(smsg);
		}
		// Otherwise, send message of object reward to client
		else if (count > 1)
		{
			SystemMessagePacket smsg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
			smsg.Params.addItemName(item);
			smsg.Params.addLong(count);
			player.sendPacket(smsg);
		}
		else
		{
			SystemMessagePacket smsg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_ACQUIRED_S1);
			smsg.Params.addItemName(item);
			player.sendPacket(smsg);
		}
		// send packets
		player.sendPacket(new ExUserInfoInventoryWeightPacket(player));
		player.sendPacket(new ExAdenaInvenCountPacket(player));
	}
	
	/**
	 * Give item/reward to the player
	 * @param player
	 * @param itemId
	 * @param count
	 */
	public static void giveItems(Player player, int itemId, long count)
	{
		giveItems(player, itemId, count, 0, false);
	}
	
	/**
	 * Give item/reward to the player
	 * @param player
	 * @param itemId
	 * @param count
	 * @param playSound
	 */
	public static void giveItems(Player player, int itemId, long count, bool playSound)
	{
		giveItems(player, itemId, count, 0, playSound);
	}
	
	/**
	 * Give item/reward to the player
	 * @param player
	 * @param holder
	 */
	protected void giveItems(Player player, ItemHolder holder)
	{
		giveItems(player, holder.getId(), holder.getCount());
	}
	
	/**
	 * @param player
	 * @param itemId
	 * @param count
	 * @param enchantlevel
	 * @param playSound
	 */
	public static void giveItems(Player player, int itemId, long count, int enchantlevel, bool playSound)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		
		if (count <= 0)
		{
			return;
		}
		
		// Add items to player's inventory
		Item item = player.getInventory().addItem("Quest", itemId, count, player, player.getTarget());
		if (item == null)
		{
			return;
		}
		
		// set enchant level for item if that item is not adena
		if ((enchantlevel > 0) && (itemId != Inventory.ADENA_ID))
		{
			item.setEnchantLevel(enchantlevel);
		}
		
		if (playSound)
		{
			AbstractScript.playSound(player, QuestSound.ITEMSOUND_QUEST_ITEMGET);
		}
		sendItemGetMessage(player, item, count);
	}
	
	/**
	 * @param player
	 * @param itemId
	 * @param count
	 * @param attributeType
	 * @param attributeValue
	 */
	public static void giveItems(Player player, int itemId, long count, AttributeType attributeType, int attributeValue)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		
		if (count <= 0)
		{
			return;
		}
		
		// Add items to player's inventory
		Item item = player.getInventory().addItem("Quest", itemId, count, player, player.getTarget());
		if (item == null)
		{
			return;
		}
		
		// set enchant level for item if that item is not adena
		if ((attributeType != null) && (attributeValue > 0))
		{
			item.setAttribute(new AttributeHolder(attributeType, attributeValue), true);
			if (item.isEquipped())
			{
				// Recalculate all stats
				player.getStat().recalculateStats(true);
			}
			
			InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(item, ItemChangeType.MODIFIED));
			player.sendInventoryUpdate(iu);
		}
		
		sendItemGetMessage(player, item, count);
	}
	
	/**
	 * Give the specified player a set amount of items if he is lucky enough.<br>
	 * Not recommended to use this for non-stacking items.
	 * @param player the player to give the item(s) to
	 * @param itemId the ID of the item to give
	 * @param amountToGive the amount of items to give
	 * @param limit the maximum amount of items the player can have. Won't give more if this limit is reached. 0 - no limit.
	 * @param dropChance the drop chance as a decimal digit from 0 to 1
	 * @param playSound if true, plays ItemSound.quest_itemget when items are given and ItemSound.quest_middle when the limit is reached
	 * @return {@code true} if limit > 0 and the limit was reached or if limit <= 0 and items were given; {@code false} in all other cases
	 */
	public static bool giveItemRandomly(Player player, int itemId, long amountToGive, long limit, double dropChance, bool playSound)
	{
		return giveItemRandomly(player, null, itemId, amountToGive, amountToGive, limit, dropChance, playSound);
	}
	
	/**
	 * Give the specified player a set amount of items if he is lucky enough.<br>
	 * Not recommended to use this for non-stacking items.
	 * @param player the player to give the item(s) to
	 * @param npc the NPC that "dropped" the item (can be null)
	 * @param itemId the ID of the item to give
	 * @param amountToGive the amount of items to give
	 * @param limit the maximum amount of items the player can have. Won't give more if this limit is reached. 0 - no limit.
	 * @param dropChance the drop chance as a decimal digit from 0 to 1
	 * @param playSound if true, plays ItemSound.quest_itemget when items are given and ItemSound.quest_middle when the limit is reached
	 * @return {@code true} if limit > 0 and the limit was reached or if limit <= 0 and items were given; {@code false} in all other cases
	 */
	public static bool giveItemRandomly(Player player, Npc npc, int itemId, long amountToGive, long limit, double dropChance, bool playSound)
	{
		return giveItemRandomly(player, npc, itemId, amountToGive, amountToGive, limit, dropChance, playSound);
	}
	
	/**
	 * Give the specified player a random amount of items if he is lucky enough.<br>
	 * Not recommended to use this for non-stacking items.
	 * @param player the player to give the item(s) to
	 * @param npc the NPC that "dropped" the item (can be null)
	 * @param itemId the ID of the item to give
	 * @param minAmount the minimum amount of items to give
	 * @param maxAmount the maximum amount of items to give (will give a random amount between min/maxAmount multiplied by quest rates)
	 * @param limit the maximum amount of items the player can have. Won't give more if this limit is reached. 0 - no limit.
	 * @param dropChance the drop chance as a decimal digit from 0 to 1
	 * @param playSound if true, plays ItemSound.quest_itemget when items are given and ItemSound.quest_middle when the limit is reached
	 * @return {@code true} if limit > 0 and the limit was reached or if limit <= 0 and items were given; {@code false} in all other cases
	 */
	public static bool giveItemRandomly(Player player, Npc npc, int itemId, long minAmount, long maxAmount, long limit, double dropChance, bool playSound)
	{
		if (player.isSimulatingTalking())
		{
			return false;
		}
		
		long currentCount = getQuestItemsCount(player, itemId);
		if ((limit > 0) && (currentCount >= limit))
		{
			return true;
		}
		
		long minAmountWithBonus = (long) (minAmount * Config.RATE_QUEST_DROP);
		long maxAmountWithBonus = (long) (maxAmount * Config.RATE_QUEST_DROP);
		double dropChanceWithBonus = dropChance * Config.RATE_QUEST_DROP; // TODO separate configs for rate and amount
		if ((npc != null) && Config.CHAMPION_ENABLE && npc.isChampion())
		{
			if ((itemId == Inventory.ADENA_ID) || (itemId == Inventory.ANCIENT_ADENA_ID))
			{
				dropChanceWithBonus *= Config.CHAMPION_ADENAS_REWARDS_CHANCE;
				minAmountWithBonus = (long)(minAmountWithBonus * Config.CHAMPION_ADENAS_REWARDS_AMOUNT);
				maxAmountWithBonus = (long)(maxAmountWithBonus * Config.CHAMPION_ADENAS_REWARDS_AMOUNT);
			}
			else
			{
				dropChanceWithBonus *= Config.CHAMPION_REWARDS_CHANCE;
				minAmountWithBonus = (long)(minAmountWithBonus * Config.CHAMPION_REWARDS_AMOUNT);
				maxAmountWithBonus = (long)(maxAmountWithBonus * Config.CHAMPION_REWARDS_AMOUNT);
			}
		}
		
		long amountToGive = (minAmountWithBonus == maxAmountWithBonus) ? minAmountWithBonus : Rnd.get(minAmountWithBonus, maxAmountWithBonus);
		double random = Rnd.nextDouble();
		// Inventory slot check (almost useless for non-stacking items)
		if ((dropChanceWithBonus >= random) && (amountToGive > 0) && player.getInventory().validateCapacityByItemId(itemId))
		{
			if ((limit > 0) && ((currentCount + amountToGive) > limit))
			{
				amountToGive = limit - currentCount;
			}
			
			// Give the item to player
			if (player.addItem("Quest", itemId, amountToGive, npc, true) != null)
			{
				// limit reached (if there is no limit, this block doesn't execute)
				if ((currentCount + amountToGive) == limit)
				{
					if (playSound)
					{
						AbstractScript.playSound(player, QuestSound.ITEMSOUND_QUEST_MIDDLE);
					}
					return true;
				}
				
				if (playSound)
				{
					AbstractScript.playSound(player, QuestSound.ITEMSOUND_QUEST_ITEMGET);
				}
				
				// if there is no limit, return true every time an item is given
				if (limit <= 0)
				{
					return true;
				}
			}
		}
		return false;
	}
	
	/**
	 * Take an amount of a specified item from player's inventory.
	 * @param player the player whose item to take
	 * @param itemId the ID of the item to take
	 * @param amountValue the amount to take
	 * @return {@code true} if any items were taken, {@code false} otherwise
	 */
	public static bool takeItems(Player player, int itemId, long amountValue)
	{
		if (player.isSimulatingTalking())
		{
			return false;
		}
		
		// Get object item from player's inventory list
		Item item = player.getInventory().getItemByItemId(itemId);
		if (item == null)
		{
			return false;
		}
		
		// Tests on count value in order not to have negative value
		long amount = amountValue;
		if ((amountValue < 0) || (amountValue > item.getCount()))
		{
			amount = item.getCount();
		}
		
		// Destroy the quantity of items wanted
		if (item.isEquipped())
		{
			List<ItemInfo> items = new List<ItemInfo>();
			foreach (Item itm in player.getInventory().unEquipItemInBodySlotAndRecord(item.getTemplate().getBodyPart()))
			{
				items.Add(new ItemInfo(itm, ItemChangeType.MODIFIED));
			}

			InventoryUpdatePacket iu = new InventoryUpdatePacket(items);
			player.sendInventoryUpdate(iu);
			player.broadcastUserInfo();
		}
		
		return player.destroyItemByItemId("Quest", itemId, amount, player, true);
	}
	
	/**
	 * Take a set amount of a specified item from player's inventory.
	 * @param player the player whose item to take
	 * @param holder the {@link ItemHolder} object containing the ID and count of the item to take
	 * @return {@code true} if the item was taken, {@code false} otherwise
	 */
	protected static bool takeItem(Player player, ItemHolder holder)
	{
		if (holder == null)
		{
			return false;
		}
		return takeItems(player, holder.getId(), holder.getCount());
	}
	
	/**
	 * Take a set amount of all specified items from player's inventory.
	 * @param player the player whose items to take
	 * @param itemList the list of {@link ItemHolder} objects containing the IDs and counts of the items to take
	 * @return {@code true} if all items were taken, {@code false} otherwise
	 */
	protected static bool takeAllItems(Player player, params ItemHolder[] itemList)
	{
		if (player.isSimulatingTalking())
		{
			return false;
		}
		if ((itemList == null) || (itemList.Length == 0))
		{
			return false;
		}
		// first check if the player has all items to avoid taking half the items from the list
		if (!hasAllItems(player, true, itemList))
		{
			return false;
		}
		foreach (ItemHolder item in itemList)
		{
			// this should never be false, but just in case
			if (!takeItem(player, item))
			{
				return false;
			}
		}
		return true;
	}
	
	/**
	 * Take an amount of all specified items from player's inventory.
	 * @param player the player whose items to take
	 * @param amount the amount to take of each item
	 * @param itemIds a list or an array of IDs of the items to take
	 * @return {@code true} if all items were taken, {@code false} otherwise
	 */
	public static bool takeItemsByIds(Player player, long amount, params int[] itemIds)
	{
		if (player.isSimulatingTalking())
		{
			return false;
		}
		
		bool check = true;
		if (itemIds != null)
		{
			foreach (int item in itemIds)
			{
				check &= takeItems(player, item, amount);
			}
		}
		return check;
	}
	
	public static void playSound(Instance world, string sound)
	{
		world.broadcastPacket().SendPackets(new PlaySoundPacket(sound));
	}
	
	/**
	 * Send a packet in order to play a sound to the player.
	 * @param player the player whom to send the packet
	 * @param sound the name of the sound to play
	 */
	public static void playSound(Player player, string sound)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		
		player.sendPacket(new PlaySoundPacket(sound));
	}
	
	/**
	 * Send a packet in order to play a sound to the player.
	 * @param player the player whom to send the packet
	 * @param sound the {@link QuestSound} object of the sound to play
	 */
	public static void playSound(Player player, QuestSound sound)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		
		player.sendPacket(new PlaySoundPacket(sound.GetSoundName()));
	}
	
	/**
	 * Add EXP and SP as quest reward.
	 * @param player the player whom to reward with the EXP/SP
	 * @param exp the amount of EXP to give to the player
	 * @param sp the amount of SP to give to the player
	 */
	public static void addExpAndSp(Player player, long exp, int sp)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		
		long addExp = exp;
		long addSp = sp;
		
		// Premium rates
		if (player.hasPremiumStatus())
		{
			addExp = (long)(addExp * Config.PREMIUM_RATE_QUEST_XP);
			addSp = (long)(addSp * Config.PREMIUM_RATE_QUEST_SP);
		}
		
		player.addExpAndSp((long) player.getStat().getValue(Stat.EXPSP_RATE, (addExp * Config.RATE_QUEST_REWARD_XP)), (int) player.getStat().getValue(Stat.EXPSP_RATE, (addSp * Config.RATE_QUEST_REWARD_SP)));
		PcCafePointsManager.getInstance().givePcCafePoint(player, (long) (addExp * Config.RATE_QUEST_REWARD_XP));
	}
	
	/**
	 * Get a random integer from 0 (inclusive) to {@code max} (exclusive).<br>
	 * Use this method instead of importing {@link org.l2jmobius.commons.util.Rnd} utility.
	 * @param max the maximum value for randomization
	 * @return a random integer number from 0 to {@code max - 1}
	 */
	public static int getRandom(int max)
	{
		return Rnd.get(max);
	}
	
	/**
	 * Get a random integer from {@code min} (inclusive) to {@code max} (inclusive).<br>
	 * Use this method instead of importing {@link org.l2jmobius.commons.util.Rnd} utility.
	 * @param min the minimum value for randomization
	 * @param max the maximum value for randomization
	 * @return a random integer number from {@code min} to {@code max}
	 */
	public static int getRandom(int min, int max)
	{
		return Rnd.get(min, max);
	}
	
	/**
	 * Get a random bool.<br>
	 * Use this method instead of importing {@link org.l2jmobius.commons.util.Rnd} utility.
	 * @return {@code true} or {@code false} randomly
	 */
	public static bool getRandomBoolean()
	{
		return Rnd.nextBoolean();
	}
	
	/**
	 * Get a random entry.
	 * @param <T>
	 * @param array of values.
	 * @return one value from array.
	 */
	public static T getRandomEntry<T>(params T[] array)
	{
		if (array.Length == 0)
		{
			return default;
		}

		return array[getRandom(array.Length)];
	}
	
	/**
	 * Get a random entry.
	 * @param <T>
	 * @param list of values.
	 * @return one value from list.
	 */
	public static T getRandomEntry<T>(List<T> list)
	{
		if (list.isEmpty())
		{
			return default;
		}
		
		return list.get(getRandom(list.size()));
	}
	
	/**
	 * Get a random entry.
	 * @param array of Integers.
	 * @return one int from array.
	 */
	public static int getRandomEntry(params int[] array)
	{
		return array[getRandom(array.Length)];
	}
	
	/**
	 * Get the ID of the item equipped in the specified inventory slot of the player.
	 * @param player the player whose inventory to check
	 * @param slot the location in the player's inventory to check
	 * @return the ID of the item equipped in the specified inventory slot or 0 if the slot is empty or item is {@code null}.
	 */
	public static int getItemEquipped(Player player, int slot)
	{
		return player.getInventory().getPaperdollItemId(slot);
	}
	
	/**
	 * @return the number of ticks from the {@link org.l2jmobius.gameserver.taskmanager.GameTimeTaskManager}.
	 */
	public static int getGameTicks()
	{
		return GameTimeTaskManager.getInstance().getGameTicks();
	}
	
	/**
	 * Execute a procedure for each player depending on the parameters.
	 * @param player the player on which the procedure will be executed
	 * @param npc the related NPC
	 * @param isSummon {@code true} if the event that called this method was originated by the player's summon, {@code false} otherwise
	 * @param includeParty if {@code true}, #actionForEachPlayer(Player, Npc, bool) will be called with the player's party members
	 * @param includeCommandChannel if {@code true}, {@link #actionForEachPlayer(Player, Npc, bool)} will be called with the player's command channel members
	 * @see #actionForEachPlayer(Player, Npc, bool)
	 */
	public void executeForEachPlayer(Player player, Npc npc, bool isSummon, bool includeParty, bool includeCommandChannel)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		if ((includeParty || includeCommandChannel) && player.isInParty())
		{
			if (includeCommandChannel && player.getParty().isInCommandChannel())
			{
				player.getParty().getCommandChannel().forEachMember(member =>
				{
					actionForEachPlayer(member, npc, isSummon);
					return true;
				});
			}
			else if (includeParty)
			{
				player.getParty().forEachMember(member =>
				{
					actionForEachPlayer(member, npc, isSummon);
					return true;
				});
			}
		}
		else
		{
			actionForEachPlayer(player, npc, isSummon);
		}
	}
	
	/**
	 * Overridable method called from {@link #executeForEachPlayer(Player, Npc, bool, bool, bool)}
	 * @param player the player on which the action will be run
	 * @param npc the NPC related to this action
	 * @param isSummon {@code true} if the event that called this method was originated by the player's summon
	 */
	public void actionForEachPlayer(Player player, Npc npc, bool isSummon)
	{
		// To be overridden in quest scripts.
	}
	
	/**
	 * Open a door if it is present on the instance and its not open.
	 * @param doorId the ID of the door to open
	 * @param instanceId the ID of the instance the door is in (0 if the door is not not inside an instance)
	 */
	public void openDoor(int doorId, int instanceId)
	{
		Door door = getDoor(doorId, instanceId);
		if (door == null)
		{
			_logger.Warn(GetType().Name + ": called openDoor(" + doorId + ", " + instanceId +
			            "); but door wasnt found!");
		}
		else if (!door.isOpen())
		{
			door.openMe();
		}
	}
	
	/**
	 * Close a door if it is present in a specified the instance and its open.
	 * @param doorId the ID of the door to close
	 * @param instanceId the ID of the instance the door is in (0 if the door is not not inside an instance)
	 */
	public void closeDoor(int doorId, int instanceId)
	{
		Door door = getDoor(doorId, instanceId);
		if (door == null)
		{
			_logger.Warn(
				GetType().Name + ": called closeDoor(" + doorId + ", " + instanceId + "); but door wasnt found!");
		}
		else if (door.isOpen())
		{
			door.closeMe();
		}
	}
	
	/**
	 * Retrieve a door from an instance or the real world.
	 * @param doorId the ID of the door to get
	 * @param instanceId the ID of the instance the door is in (0 if the door is not not inside an instance)
	 * @return the found door or {@code null} if no door with that ID and instance ID was found
	 */
	public Door getDoor(int doorId, int instanceId)
	{
		Door door;
		Instance instance = InstanceManager.getInstance().getInstance(instanceId);
		if (instance != null)
		{
			door = instance.getDoor(doorId);
		}
		else
		{
			door = DoorData.getInstance().getDoor(doorId);
		}

		return door;
	}
	
	/**
	 * Monster is running and attacking the playable.
	 * @param npc the NPC that performs the attack
	 * @param playable the player
	 */
	protected void addAttackPlayerDesire(Npc npc, Playable playable)
	{
		addAttackPlayerDesire(npc, playable, 999);
	}
	
	/**
	 * Monster is running and attacking the target.
	 * @param npc the NPC that performs the attack
	 * @param target the target of the attack
	 * @param desire the desire to perform the attack
	 */
	protected void addAttackPlayerDesire(Npc npc, Playable target, int desire)
	{
		if (npc.isAttackable())
		{
			((Attackable) npc).addDamageHate(target, 0, desire);
		}
		npc.setRunning();
		npc.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, target);
	}
	
	/**
	 * Monster is running and attacking the target.
	 * @param npc the NPC that performs the attack
	 * @param target the target of the attack
	 */
	protected void addAttackDesire(Npc npc, Creature target)
	{
		npc.setRunning();
		npc.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, target);
	}
	
	/**
	 * Adds desire to move to the given NPC.
	 * @param npc the NPC
	 * @param loc the location
	 * @param desire the desire
	 */
	protected void addMoveToDesire(Npc npc, Location3D destination, int desire)
	{
		npc.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, destination);
	}
	
	/**
	 * Instantly cast a skill upon the given target.
	 * @param npc the caster NPC
	 * @param target the target of the cast
	 * @param skill the skill to cast
	 */
	protected void castSkill(Npc npc, Playable target, SkillHolder skill)
	{
		npc.setTarget(target);
		npc.doCast(skill.getSkill());
	}
	
	/**
	 * Instantly cast a skill upon the given target.
	 * @param npc the caster NPC
	 * @param target the target of the cast
	 * @param skill the skill to cast
	 */
	protected void castSkill(Npc npc, Playable target, Skill skill)
	{
		npc.setTarget(target);
		npc.doCast(skill);
	}
	
	/**
	 * Adds the desire to cast a skill to the given NPC.
	 * @param npc the NPC whom cast the skill
	 * @param target the skill target
	 * @param skill the skill to cast
	 * @param desire the desire to cast the skill
	 */
	protected void addSkillCastDesire(Npc npc, WorldObject target, SkillHolder skill, int desire)
	{
		addSkillCastDesire(npc, target, skill.getSkill(), desire);
	}
	
	/**
	 * Adds the desire to cast a skill to the given NPC.
	 * @param npc the NPC whom cast the skill
	 * @param target the skill target
	 * @param skill the skill to cast
	 * @param desire the desire to cast the skill
	 */
	protected void addSkillCastDesire(Npc npc, WorldObject target, Skill skill, int desire)
	{
		if (npc.isAttackable() && (target != null) && target.isCreature())
		{
			((Attackable) npc).addDamageHate((Creature) target, 0, desire);
		}
		npc.setTarget(target != null ? target : npc);
		npc.doCast(skill);
	}
	
	/**
	 * Sends the special camera packet to the player.
	 * @param player the player
	 * @param creature the watched creature
	 * @param force
	 * @param angle1
	 * @param angle2
	 * @param time
	 * @param range
	 * @param duration
	 * @param relYaw
	 * @param relPitch
	 * @param isWide
	 * @param relAngle
	 */
	public static void specialCamera(Player player, Creature creature, int force, int angle1, int angle2, int time, int range, int duration, int relYaw, int relPitch, int isWide, int relAngle)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		player.sendPacket(new SpecialCameraPacket(creature, force, angle1, angle2, time, range, duration, relYaw, relPitch, isWide, relAngle));
	}
	
	/**
	 * Sends the special camera packet to the player.
	 * @param player
	 * @param creature
	 * @param force
	 * @param angle1
	 * @param angle2
	 * @param time
	 * @param duration
	 * @param relYaw
	 * @param relPitch
	 * @param isWide
	 * @param relAngle
	 */
	public static void specialCameraEx(Player player, Creature creature, int force, int angle1, int angle2, int time, int duration, int relYaw, int relPitch, int isWide, int relAngle)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		player.sendPacket(new SpecialCameraPacket(creature, player, force, angle1, angle2, time, duration, relYaw, relPitch, isWide, relAngle));
	}
	
	/**
	 * Sends the special camera packet to the player.
	 * @param player
	 * @param creature
	 * @param force
	 * @param angle1
	 * @param angle2
	 * @param time
	 * @param range
	 * @param duration
	 * @param relYaw
	 * @param relPitch
	 * @param isWide
	 * @param relAngle
	 * @param unk
	 */
	public static void specialCamera3(Player player, Creature creature, int force, int angle1, int angle2, int time, int range, int duration, int relYaw, int relPitch, int isWide, int relAngle, int unk)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		player.sendPacket(new SpecialCameraPacket(creature, force, angle1, angle2, time, range, duration, relYaw, relPitch, isWide, relAngle, unk));
	}
	
	public static void specialCamera(Instance world, Creature creature, int force, int angle1, int angle2, int time, int range, int duration, int relYaw, int relPitch, int isWide, int relAngle, int unk)
	{
		world.broadcastPacket().SendPackets(new SpecialCameraPacket(creature, force, angle1, angle2, time, range, duration, relYaw, relPitch, isWide, relAngle, unk));
	}
	
	/**
	 * @param player
	 * @param x
	 * @param y
	 * @param z
	 */
	public static void addRadar(Player player, int x, int y, int z)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		player.getRadar().addMarker(x, y, z);
	}
	
	/**
	 * @param player
	 * @param x
	 * @param y
	 * @param z
	 */
	public void removeRadar(Player player, int x, int y, int z)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		player.getRadar().removeMarker(x, y, z);
	}
	
	/**
	 * @param player
	 */
	public void clearRadar(Player player)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}
		player.getRadar().removeAllMarkers();
	}
	
	/**
	 * Play scene for Player.
	 * @param player the player
	 * @param movie the movie
	 */
	public void playMovie(Player player, Movie movie)
	{
		if (player.isSimulatingTalking())
		{
			return;
		}

		new MovieHolder([player], movie);
	}
	
	/**
	 * Play scene for all Player inside list.
	 * @param players list with Player
	 * @param movie the movie
	 */
	public void playMovie(List<Player> players, Movie movie)
	{
		new MovieHolder(players, movie);
	}
	
	/**
	 * Play scene for all Player inside set.
	 * @param players set with Player
	 * @param movie the movie
	 */
	public void playMovie(Set<Player> players, Movie movie)
	{
		new MovieHolder(players.ToList(), movie);
	}
	
	/**
	 * Play scene for all Player inside instance.
	 * @param instance Instance object
	 * @param movie the movie
	 */
	public void playMovie(Instance instance, Movie movie)
	{
		if (instance != null)
		{
			foreach (Player player in instance.getPlayers())
			{
				if ((player != null) && (player.getInstanceWorld() == instance))
				{
					playMovie(player, movie);
				}
			}
		}
	}
}