using System.Collections.Immutable;
using L2Dn.GameServer.Model.Events.Impl;
using L2Dn.GameServer.Model.Events.Impl.Clans;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Events.Impl.Instances;
using L2Dn.GameServer.Model.Events.Impl.Items;
using L2Dn.GameServer.Model.Events.Impl.Olympiads;
using L2Dn.GameServer.Model.Events.Impl.Sieges;
using L2Dn.GameServer.Model.Events.Returns;

namespace L2Dn.GameServer.Model.Events;

/**
 * @author UnAfraid
 */
public enum EventType
{
	// Attackable events
	ON_ATTACKABLE_AGGRO_RANGE_ENTER,
	ON_ATTACKABLE_ATTACK,
	ON_ATTACKABLE_FACTION_CALL,
	ON_ATTACKABLE_KILL,
	
	// Castle events
	ON_CASTLE_SIEGE_FINISH,
	ON_CASTLE_SIEGE_OWNER_CHANGE,
	ON_CASTLE_SIEGE_START,
	
	// Clan events
	ON_CLAN_WAR_FINISH,
	ON_CLAN_WAR_START,
	
	// Creature events
	ON_CREATURE_ATTACK,
	ON_CREATURE_ATTACK_AVOID,
	ON_CREATURE_ATTACKED,
	ON_CREATURE_DAMAGE_RECEIVED,
	ON_CREATURE_DAMAGE_DEALT,
	ON_CREATURE_HP_CHANGE,
	ON_CREATURE_DEATH,
	ON_CREATURE_KILLED,
	ON_CREATURE_SEE,
	ON_CREATURE_SKILL_USE,
	ON_CREATURE_SKILL_FINISH_CAST,
	ON_CREATURE_TELEPORT,
	ON_CREATURE_TELEPORTED,
	ON_CREATURE_ZONE_ENTER,
	ON_CREATURE_ZONE_EXIT,
	
	// Fortress events
	ON_FORT_SIEGE_FINISH,
	ON_FORT_SIEGE_START,
	
	// Item events
	ON_ITEM_BYPASS_EVENT,
	ON_ITEM_CREATE,
	ON_ITEM_USE,
	ON_ITEM_TALK,
	ON_ITEM_PURGE_REWARD,
	
	// NPC events
	ON_NPC_CAN_BE_SEEN,
	ON_NPC_EVENT_RECEIVED,
	ON_NPC_FIRST_TALK,
	ON_NPC_HATE,
	ON_NPC_MOVE_FINISHED,
	ON_NPC_MOVE_NODE_ARRIVED,
	ON_NPC_MOVE_ROUTE_FINISHED,
	ON_NPC_QUEST_START,
	ON_NPC_SKILL_FINISHED,
	ON_NPC_SKILL_SEE,
	ON_NPC_SPAWN,
	ON_NPC_TALK,
	ON_NPC_TELEPORT,
	ON_NPC_MANOR_BYPASS,
	ON_NPC_MENU_SELECT,
	ON_NPC_DESPAWN,
	ON_NPC_TELEPORT_REQUEST,
	
	// Olympiad events
	ON_OLYMPIAD_MATCH_RESULT,
	
	// Playable events
	ON_PLAYABLE_EXP_CHANGED,
	
	// Player events
	ON_PLAYER_AUGMENT,
	ON_PLAYER_BYPASS,
	ON_PLAYER_CALL_TO_CHANGE_CLASS,
	ON_PLAYER_CHAT,
	ON_PLAYER_ABILITY_POINTS_CHANGED,
	// Clan events
	ON_PLAYER_CLAN_CREATE,
	ON_PLAYER_CLAN_DESTROY,
	ON_PLAYER_CLAN_JOIN,
	ON_PLAYER_CLAN_LEADER_CHANGE,
	ON_PLAYER_CLAN_LEFT,
	ON_PLAYER_CLAN_LEVELUP,
	// Clan warehouse events
	ON_PLAYER_CLAN_WH_ITEM_ADD,
	ON_PLAYER_CLAN_WH_ITEM_DESTROY,
	ON_PLAYER_CLAN_WH_ITEM_TRANSFER,
	ON_PLAYER_CREATE,
	ON_PLAYER_DELETE,
	ON_PLAYER_DLG_ANSWER,
	ON_PLAYER_FAME_CHANGED,
	ON_PLAYER_FISHING,
	// Henna events
	ON_PLAYER_HENNA_ADD,
	ON_PLAYER_HENNA_REMOVE,
	// Inventory events
	ON_PLAYER_ITEM_ADD,
	ON_PLAYER_ITEM_DESTROY,
	ON_PLAYER_ITEM_DROP,
	ON_PLAYER_ITEM_PICKUP,
	ON_PLAYER_ITEM_TRANSFER,
	ON_PLAYER_ITEM_EQUIP,
	ON_PLAYER_ITEM_UNEQUIP,
	// Mentoring events
	ON_PLAYER_MENTEE_ADD,
	ON_PLAYER_MENTEE_LEFT,
	ON_PLAYER_MENTEE_REMOVE,
	ON_PLAYER_MENTEE_STATUS,
	ON_PLAYER_MENTOR_STATUS,
	// Other player events
	ON_PLAYER_REPUTATION_CHANGED,
	ON_PLAYER_LEVEL_CHANGED,
	ON_PLAYER_LOGIN,
	ON_PLAYER_LOGOUT,
	ON_PLAYER_LOAD,
	ON_PLAYER_PK_CHANGED,
	ON_PLAYER_PRESS_TUTORIAL_MARK,
	ON_PLAYER_MOVE_REQUEST,
	ON_PLAYER_PROFESSION_CHANGE,
	ON_PLAYER_PROFESSION_CANCEL,
	ON_PLAYER_CHANGE_TO_AWAKENED_CLASS,
	ON_PLAYER_PVP_CHANGED,
	ON_PLAYER_PVP_KILL,
	ON_PLAYER_RESTORE,
	ON_PLAYER_SELECT,
	ON_PLAYER_SOCIAL_ACTION,
	ON_PLAYER_SKILL_LEARN,
	ON_PLAYER_SUMMON_SPAWN,
	ON_PLAYER_SUMMON_TALK,
	ON_PLAYER_TAKE_HERO,
	ON_PLAYER_TRANSFORM,
	ON_PLAYER_SUB_CHANGE,
	ON_PLAYER_QUEST_ACCEPT,
	ON_PLAYER_QUEST_ABORT,
	ON_PLAYER_QUEST_COMPLETE,
	ON_PLAYER_SUMMON_AGATHION,
	ON_PLAYER_UNSUMMON_AGATHION,
	
	// Trap events
	ON_TRAP_ACTION,
	
	// Elemental spirit events
	ON_ELEMENTAL_SPIRIT_UPGRADE,
	ON_ELEMENTAL_SPIRIT_LEARN,
	
	// Instance events
	ON_INSTANCE_CREATED,
	ON_INSTANCE_DESTROY,
	ON_INSTANCE_ENTER,
	ON_INSTANCE_LEAVE,
	ON_INSTANCE_STATUS_CHANGE,
	
	// Server events
	ON_SERVER_START,
	ON_DAY_NIGHT_CHANGE
}

public static class EventTypeUtil
{
	private static readonly Type?[] _classTypes;
	private static readonly ImmutableArray<Type>[] _returnTypes;

	static EventTypeUtil()
	{
		int count = (int)Enum.GetValues<EventType>().Max() + 1;
		_classTypes = new Type?[count];
		_returnTypes = new ImmutableArray<Type>[count];

		foreach (EventType eventType in Enum.GetValues<EventType>())
		{
			_classTypes[(int)eventType] = GetEventClassPrivate(eventType);
			_returnTypes[(int)eventType] = GetReturnTypesPrivate(eventType).ToImmutableArray();
		}
	}

	public static Type? GetEventClass(this EventType eventType)
	{
		if (eventType >= 0 && (int)eventType < _classTypes.Length)
			return _classTypes[(int)eventType];

		throw new ArgumentOutOfRangeException();
	}

	public static ImmutableArray<Type> GetReturnTypes(this EventType eventType)
	{
		if (eventType >= 0 && (int)eventType < _classTypes.Length)
			return _returnTypes[(int)eventType];

		throw new ArgumentOutOfRangeException();
	}
	
	private static Type? GetEventClassPrivate(this EventType eventType)
	{
		// TODO: move parameters to enum as attributes 
		return eventType switch
		{
			// Attackable events
			EventType.ON_ATTACKABLE_AGGRO_RANGE_ENTER => typeof(OnAttackableAggroRangeEnter),
			EventType.ON_ATTACKABLE_ATTACK => typeof(OnAttackableAttack),
			EventType.ON_ATTACKABLE_FACTION_CALL => typeof(OnAttackableFactionCall),
			EventType.ON_ATTACKABLE_KILL => typeof(OnAttackableKill),

			// Castle events
			EventType.ON_CASTLE_SIEGE_FINISH => typeof(OnCastleSiegeFinish),
			EventType.ON_CASTLE_SIEGE_OWNER_CHANGE => typeof(OnCastleSiegeOwnerChange),
			EventType.ON_CASTLE_SIEGE_START => typeof(OnCastleSiegeStart),

			// Clan events
			EventType.ON_CLAN_WAR_FINISH => typeof(OnClanWarFinish),
			EventType.ON_CLAN_WAR_START => typeof(OnClanWarStart),

			// Creature events
			EventType.ON_CREATURE_ATTACK => typeof(OnCreatureAttack),
			EventType.ON_CREATURE_ATTACK_AVOID => typeof(OnCreatureAttackAvoid),
			EventType.ON_CREATURE_ATTACKED => typeof(OnCreatureAttacked),
			EventType.ON_CREATURE_DAMAGE_RECEIVED => typeof(OnCreatureDamageReceived),
			EventType.ON_CREATURE_DAMAGE_DEALT => typeof(OnCreatureDamageDealt),
			EventType.ON_CREATURE_HP_CHANGE => typeof(OnCreatureHpChange),
			EventType.ON_CREATURE_DEATH => typeof(OnCreatureDeath),
			EventType.ON_CREATURE_KILLED => typeof(OnCreatureKilled),
			EventType.ON_CREATURE_SEE => typeof(OnCreatureSee),
			EventType.ON_CREATURE_SKILL_USE => typeof(OnCreatureSkillUse),
			EventType.ON_CREATURE_SKILL_FINISH_CAST => typeof(OnCreatureSkillFinishCast),
			EventType.ON_CREATURE_TELEPORT => typeof(OnCreatureTeleport),
			EventType.ON_CREATURE_TELEPORTED => typeof(OnCreatureTeleported),
			EventType.ON_CREATURE_ZONE_ENTER => typeof(OnCreatureZoneEnter),
			EventType.ON_CREATURE_ZONE_EXIT => typeof(OnCreatureZoneExit),

			// Fortress events
			EventType.ON_FORT_SIEGE_FINISH => typeof(OnFortSiegeFinish),
			EventType.ON_FORT_SIEGE_START => typeof(OnFortSiegeStart),

			// Item events
			EventType.ON_ITEM_BYPASS_EVENT => typeof(OnItemBypassEvent),
			EventType.ON_ITEM_CREATE => typeof(OnItemCreate),
			EventType.ON_ITEM_USE => typeof(OnItemUse),
			EventType.ON_ITEM_TALK => typeof(OnItemTalk),
			EventType.ON_ITEM_PURGE_REWARD => typeof(OnItemPurgeReward),

			// NPC events
			EventType.ON_NPC_CAN_BE_SEEN => typeof(OnNpcCanBeSeen),
			EventType.ON_NPC_EVENT_RECEIVED => typeof(OnNpcEventReceived),
			EventType.ON_NPC_FIRST_TALK => typeof(OnNpcFirstTalk),
			EventType.ON_NPC_HATE => typeof(OnAttackableHate),
			EventType.ON_NPC_MOVE_FINISHED => typeof(OnNpcMoveFinished),
			EventType.ON_NPC_MOVE_NODE_ARRIVED => typeof(OnNpcMoveNodeArrived),
			EventType.ON_NPC_MOVE_ROUTE_FINISHED => typeof(OnNpcMoveRouteFinished),
			EventType.ON_NPC_QUEST_START => null,
			EventType.ON_NPC_SKILL_FINISHED => typeof(OnNpcSkillFinished),
			EventType.ON_NPC_SKILL_SEE => typeof(OnNpcSkillSee),
			EventType.ON_NPC_SPAWN => typeof(OnNpcSpawn),
			EventType.ON_NPC_TALK => null,
			EventType.ON_NPC_TELEPORT => typeof(OnNpcTeleport),
			EventType.ON_NPC_MANOR_BYPASS => typeof(OnNpcManorBypass),
			EventType.ON_NPC_MENU_SELECT => typeof(OnNpcMenuSelect),
			EventType.ON_NPC_DESPAWN => typeof(OnNpcDespawn),
			EventType.ON_NPC_TELEPORT_REQUEST => typeof(OnNpcTeleportRequest),

			// Olympiad events
			EventType.ON_OLYMPIAD_MATCH_RESULT => typeof(OnOlympiadMatchResult),

			// Playable events
			EventType.ON_PLAYABLE_EXP_CHANGED => typeof(OnPlayableExpChanged),

			// Player events
			EventType.ON_PLAYER_AUGMENT => typeof(OnPlayerAugment),
			EventType.ON_PLAYER_BYPASS => typeof(OnPlayerBypass),
			EventType.ON_PLAYER_CALL_TO_CHANGE_CLASS => typeof(OnPlayerCallToChangeClass),
			EventType.ON_PLAYER_CHAT => typeof(OnPlayerChat),
			EventType.ON_PLAYER_ABILITY_POINTS_CHANGED => typeof(OnPlayerAbilityPointsChanged),
			// Clan events
			EventType.ON_PLAYER_CLAN_CREATE => typeof(OnPlayerClanCreate),
			EventType.ON_PLAYER_CLAN_DESTROY => typeof(OnPlayerClanDestroy),
			EventType.ON_PLAYER_CLAN_JOIN => typeof(OnPlayerClanJoin),
			EventType.ON_PLAYER_CLAN_LEADER_CHANGE => typeof(OnPlayerClanLeaderChange),
			EventType.ON_PLAYER_CLAN_LEFT => typeof(OnPlayerClanLeft),
			EventType.ON_PLAYER_CLAN_LEVELUP => typeof(OnPlayerClanLvlUp),
			// Clan warehouse events
			EventType.ON_PLAYER_CLAN_WH_ITEM_ADD => typeof(OnPlayerClanWHItemAdd),
			EventType.ON_PLAYER_CLAN_WH_ITEM_DESTROY => typeof(OnPlayerClanWHItemDestroy),
			EventType.ON_PLAYER_CLAN_WH_ITEM_TRANSFER => typeof(OnPlayerClanWHItemTransfer),
			EventType.ON_PLAYER_CREATE => typeof(OnPlayerCreate),
			EventType.ON_PLAYER_DELETE => typeof(OnPlayerDelete),
			EventType.ON_PLAYER_DLG_ANSWER => typeof(OnPlayerDlgAnswer),
			EventType.ON_PLAYER_FAME_CHANGED => typeof(OnPlayerFameChanged),
			EventType.ON_PLAYER_FISHING => typeof(OnPlayerFishing),
			// Henna events
			EventType.ON_PLAYER_HENNA_ADD => typeof(OnPlayerHennaAdd),
			EventType.ON_PLAYER_HENNA_REMOVE => typeof(OnPlayerHennaRemove),
			// Inventory events
			EventType.ON_PLAYER_ITEM_ADD => typeof(OnPlayerItemAdd),
			EventType.ON_PLAYER_ITEM_DESTROY => typeof(OnPlayerItemDestroy),
			EventType.ON_PLAYER_ITEM_DROP => typeof(OnPlayerItemDrop),
			EventType.ON_PLAYER_ITEM_PICKUP => typeof(OnPlayerItemPickup),
			EventType.ON_PLAYER_ITEM_TRANSFER => typeof(OnPlayerItemTransfer),
			EventType.ON_PLAYER_ITEM_EQUIP => typeof(OnPlayerItemEquip),
			EventType.ON_PLAYER_ITEM_UNEQUIP => typeof(OnPlayerItemUnequip),
			// Mentoring events
			EventType.ON_PLAYER_MENTEE_ADD => typeof(OnPlayerMenteeAdd),
			EventType.ON_PLAYER_MENTEE_LEFT => typeof(OnPlayerMenteeLeft),
			EventType.ON_PLAYER_MENTEE_REMOVE => typeof(OnPlayerMenteeRemove),
			EventType.ON_PLAYER_MENTEE_STATUS => typeof(OnPlayerMenteeStatus),
			EventType.ON_PLAYER_MENTOR_STATUS => typeof(OnPlayerMentorStatus),
			// Other player events
			EventType.ON_PLAYER_REPUTATION_CHANGED => typeof(OnPlayerReputationChanged),
			EventType.ON_PLAYER_LEVEL_CHANGED => typeof(OnPlayerLevelChanged),
			EventType.ON_PLAYER_LOGIN => typeof(OnPlayerLogin),
			EventType.ON_PLAYER_LOGOUT => typeof(OnPlayerLogout),
			EventType.ON_PLAYER_LOAD => typeof(OnPlayerLoad),
			EventType.ON_PLAYER_PK_CHANGED => typeof(OnPlayerPKChanged),
			EventType.ON_PLAYER_PRESS_TUTORIAL_MARK => typeof(OnPlayerPressTutorialMark),
			EventType.ON_PLAYER_MOVE_REQUEST => typeof(OnPlayerMoveRequest),
			EventType.ON_PLAYER_PROFESSION_CHANGE => typeof(OnPlayerProfessionChange),
			EventType.ON_PLAYER_PROFESSION_CANCEL => typeof(OnPlayerProfessionCancel),
			EventType.ON_PLAYER_CHANGE_TO_AWAKENED_CLASS => typeof(OnPlayerChangeToAwakenedClass),
			EventType.ON_PLAYER_PVP_CHANGED => typeof(OnPlayerPvPChanged),
			EventType.ON_PLAYER_PVP_KILL => typeof(OnPlayerPvPKill),
			EventType.ON_PLAYER_RESTORE => typeof(OnPlayerRestore),
			EventType.ON_PLAYER_SELECT => typeof(OnPlayerSelect),
			EventType.ON_PLAYER_SOCIAL_ACTION => typeof(OnPlayerSocialAction),
			EventType.ON_PLAYER_SKILL_LEARN => typeof(OnPlayerSkillLearn),
			EventType.ON_PLAYER_SUMMON_SPAWN => typeof(OnPlayerSummonSpawn),
			EventType.ON_PLAYER_SUMMON_TALK => typeof(OnPlayerSummonTalk),
			EventType.ON_PLAYER_TAKE_HERO => typeof(OnPlayerTakeHero),
			EventType.ON_PLAYER_TRANSFORM => typeof(OnPlayerTransform),
			EventType.ON_PLAYER_SUB_CHANGE => typeof(OnPlayerSubChange),
			EventType.ON_PLAYER_QUEST_ACCEPT => typeof(OnPlayerQuestAccept),
			EventType.ON_PLAYER_QUEST_ABORT => typeof(OnPlayerQuestAbort),
			EventType.ON_PLAYER_QUEST_COMPLETE => typeof(OnPlayerQuestComplete),
			EventType.ON_PLAYER_SUMMON_AGATHION => typeof(OnPlayerSummonAgathion),
			EventType.ON_PLAYER_UNSUMMON_AGATHION => typeof(OnPlayerUnsummonAgathion),

			// Trap events
			EventType.ON_TRAP_ACTION => typeof(OnTrapAction),

			// Elemental spirit events
			EventType.ON_ELEMENTAL_SPIRIT_UPGRADE => typeof(OnElementalSpiritUpgrade),
			EventType.ON_ELEMENTAL_SPIRIT_LEARN => typeof(OnElementalSpiritLearn),

			// Instance events
			EventType.ON_INSTANCE_CREATED => typeof(OnInstanceCreated),
			EventType.ON_INSTANCE_DESTROY => typeof(OnInstanceDestroy),
			EventType.ON_INSTANCE_ENTER => typeof(OnInstanceEnter),
			EventType.ON_INSTANCE_LEAVE => typeof(OnInstanceLeave),
			EventType.ON_INSTANCE_STATUS_CHANGE => typeof(OnInstanceStatusChange),

			// Server events
			EventType.ON_SERVER_START => typeof(OnServerStart),
			EventType.ON_DAY_NIGHT_CHANGE => typeof(OnDayNightChange),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	private static Type[] GetReturnTypesPrivate(this EventType eventType)
	{
		return eventType switch
		{
			// Attackable events
			EventType.ON_ATTACKABLE_AGGRO_RANGE_ENTER => [typeof(void)],
			EventType.ON_ATTACKABLE_ATTACK => [typeof(void)],
			EventType.ON_ATTACKABLE_FACTION_CALL => [typeof(void)],
			EventType.ON_ATTACKABLE_KILL => [typeof(void)],

			// Castle events
			EventType.ON_CASTLE_SIEGE_FINISH => [typeof(void)],
			EventType.ON_CASTLE_SIEGE_OWNER_CHANGE => [typeof(void)],
			EventType.ON_CASTLE_SIEGE_START => [typeof(void)],

			// Clan events
			EventType.ON_CLAN_WAR_FINISH => [typeof(void)],
			EventType.ON_CLAN_WAR_START => [typeof(void)],

			// Creature events
			EventType.ON_CREATURE_ATTACK => [typeof(void), typeof(TerminateReturn)],
			EventType.ON_CREATURE_ATTACK_AVOID => [typeof(void), typeof(void)],
			EventType.ON_CREATURE_ATTACKED => [typeof(void), typeof(TerminateReturn)],
			EventType.ON_CREATURE_DAMAGE_RECEIVED => [typeof(void), typeof(DamageReturn)],
			EventType.ON_CREATURE_DAMAGE_DEALT => [typeof(void)],
			EventType.ON_CREATURE_HP_CHANGE => [typeof(void)],
			EventType.ON_CREATURE_DEATH => [typeof(void)],
			EventType.ON_CREATURE_KILLED => [typeof(void), typeof(TerminateReturn)],
			EventType.ON_CREATURE_SEE => [typeof(void)],
			EventType.ON_CREATURE_SKILL_USE => [typeof(void), typeof(TerminateReturn)],
			EventType.ON_CREATURE_SKILL_FINISH_CAST => [typeof(void)],
			EventType.ON_CREATURE_TELEPORT => [typeof(void), typeof(LocationReturn)],
			EventType.ON_CREATURE_TELEPORTED => [typeof(void)],
			EventType.ON_CREATURE_ZONE_ENTER => [typeof(void)],
			EventType.ON_CREATURE_ZONE_EXIT => [typeof(void)],

			// Fortress events
			EventType.ON_FORT_SIEGE_FINISH => [typeof(void)],
			EventType.ON_FORT_SIEGE_START => [typeof(void)],

			// Item events
			EventType.ON_ITEM_BYPASS_EVENT => [typeof(void)],
			EventType.ON_ITEM_CREATE => [typeof(void)],
			EventType.ON_ITEM_USE => [typeof(void)],
			EventType.ON_ITEM_TALK => [typeof(void)],
			EventType.ON_ITEM_PURGE_REWARD => [typeof(void)],

			// NPC events
			EventType.ON_NPC_CAN_BE_SEEN => [typeof(void), typeof(TerminateReturn)],
			EventType.ON_NPC_EVENT_RECEIVED => [typeof(void)],
			EventType.ON_NPC_FIRST_TALK => [typeof(void)],
			EventType.ON_NPC_HATE => [typeof(void), typeof(TerminateReturn)],
			EventType.ON_NPC_MOVE_FINISHED => [typeof(void)],
			EventType.ON_NPC_MOVE_NODE_ARRIVED => [typeof(void)],
			EventType.ON_NPC_MOVE_ROUTE_FINISHED => [typeof(void)],
			EventType.ON_NPC_QUEST_START => [typeof(void)],
			EventType.ON_NPC_SKILL_FINISHED => [typeof(void)],
			EventType.ON_NPC_SKILL_SEE => [typeof(void)],
			EventType.ON_NPC_SPAWN => [typeof(void)],
			EventType.ON_NPC_TALK => [typeof(void)],
			EventType.ON_NPC_TELEPORT => [typeof(void)],
			EventType.ON_NPC_MANOR_BYPASS => [typeof(void)],
			EventType.ON_NPC_MENU_SELECT => [typeof(void)],
			EventType.ON_NPC_DESPAWN => [typeof(void)],
			EventType.ON_NPC_TELEPORT_REQUEST => [typeof(void), typeof(TerminateReturn)],

			// Olympiad events
			EventType.ON_OLYMPIAD_MATCH_RESULT => [typeof(void)],

			// Playable events
			EventType.ON_PLAYABLE_EXP_CHANGED => [typeof(void), typeof(TerminateReturn)],

			// Player events
			EventType.ON_PLAYER_AUGMENT => [typeof(void)],
			EventType.ON_PLAYER_BYPASS => [typeof(void), typeof(TerminateReturn)],
			EventType.ON_PLAYER_CALL_TO_CHANGE_CLASS => [typeof(void)],
			EventType.ON_PLAYER_CHAT => [typeof(void), typeof(ChatFilterReturn)],
			EventType.ON_PLAYER_ABILITY_POINTS_CHANGED => [typeof(void)],
			// Clan events
			EventType.ON_PLAYER_CLAN_CREATE => [typeof(void)],
			EventType.ON_PLAYER_CLAN_DESTROY => [typeof(void)],
			EventType.ON_PLAYER_CLAN_JOIN => [typeof(void)],
			EventType.ON_PLAYER_CLAN_LEADER_CHANGE => [typeof(void)],
			EventType.ON_PLAYER_CLAN_LEFT => [typeof(void)],
			EventType.ON_PLAYER_CLAN_LEVELUP => [typeof(void)],
			// Clan warehouse events
			EventType.ON_PLAYER_CLAN_WH_ITEM_ADD => [typeof(void)],
			EventType.ON_PLAYER_CLAN_WH_ITEM_DESTROY => [typeof(void)],
			EventType.ON_PLAYER_CLAN_WH_ITEM_TRANSFER => [typeof(void)],
			EventType.ON_PLAYER_CREATE => [typeof(void)],
			EventType.ON_PLAYER_DELETE => [typeof(void)],
			EventType.ON_PLAYER_DLG_ANSWER => [typeof(void), typeof(TerminateReturn)],
			EventType.ON_PLAYER_FAME_CHANGED => [typeof(void)],
			EventType.ON_PLAYER_FISHING => [typeof(void)],
			// Henna events
			EventType.ON_PLAYER_HENNA_ADD => [typeof(void)],
			EventType.ON_PLAYER_HENNA_REMOVE => [typeof(void)],
			// Inventory events
			EventType.ON_PLAYER_ITEM_ADD => [typeof(void)],
			EventType.ON_PLAYER_ITEM_DESTROY => [typeof(void)],
			EventType.ON_PLAYER_ITEM_DROP => [typeof(void)],
			EventType.ON_PLAYER_ITEM_PICKUP => [typeof(void)],
			EventType.ON_PLAYER_ITEM_TRANSFER => [typeof(void)],
			EventType.ON_PLAYER_ITEM_EQUIP => [typeof(void)],
			EventType.ON_PLAYER_ITEM_UNEQUIP => [typeof(void)],
			// Mentoring events
			EventType.ON_PLAYER_MENTEE_ADD => [typeof(void)],
			EventType.ON_PLAYER_MENTEE_LEFT => [typeof(void)],
			EventType.ON_PLAYER_MENTEE_REMOVE => [typeof(void)],
			EventType.ON_PLAYER_MENTEE_STATUS => [typeof(void)],
			EventType.ON_PLAYER_MENTOR_STATUS => [typeof(void)],
			// Other player events
			EventType.ON_PLAYER_REPUTATION_CHANGED => [typeof(void)],
			EventType.ON_PLAYER_LEVEL_CHANGED => [typeof(void)],
			EventType.ON_PLAYER_LOGIN => [typeof(void)],
			EventType.ON_PLAYER_LOGOUT => [typeof(void)],
			EventType.ON_PLAYER_LOAD => [typeof(void)],
			EventType.ON_PLAYER_PK_CHANGED => [typeof(void)],
			EventType.ON_PLAYER_PRESS_TUTORIAL_MARK => [typeof(void)],
			EventType.ON_PLAYER_MOVE_REQUEST => [typeof(void), typeof(TerminateReturn)],
			EventType.ON_PLAYER_PROFESSION_CHANGE => [typeof(void)],
			EventType.ON_PLAYER_PROFESSION_CANCEL => [typeof(void)],
			EventType.ON_PLAYER_CHANGE_TO_AWAKENED_CLASS => [typeof(void)],
			EventType.ON_PLAYER_PVP_CHANGED => [typeof(void)],
			EventType.ON_PLAYER_PVP_KILL => [typeof(void)],
			EventType.ON_PLAYER_RESTORE => [typeof(void)],
			EventType.ON_PLAYER_SELECT => [typeof(void), typeof(TerminateReturn)],
			EventType.ON_PLAYER_SOCIAL_ACTION => [typeof(void)],
			EventType.ON_PLAYER_SKILL_LEARN => [typeof(void)],
			EventType.ON_PLAYER_SUMMON_SPAWN => [typeof(void)],
			EventType.ON_PLAYER_SUMMON_TALK => [typeof(void)],
			EventType.ON_PLAYER_TAKE_HERO => [typeof(void)],
			EventType.ON_PLAYER_TRANSFORM => [typeof(void)],
			EventType.ON_PLAYER_SUB_CHANGE => [typeof(void)],
			EventType.ON_PLAYER_QUEST_ACCEPT => [typeof(void)],
			EventType.ON_PLAYER_QUEST_ABORT => [typeof(void)],
			EventType.ON_PLAYER_QUEST_COMPLETE => [typeof(void)],
			EventType.ON_PLAYER_SUMMON_AGATHION => [typeof(void)],
			EventType.ON_PLAYER_UNSUMMON_AGATHION => [typeof(void)],

			// Trap events
			EventType.ON_TRAP_ACTION => [typeof(void)],

			// Elemental spirit events
			EventType.ON_ELEMENTAL_SPIRIT_UPGRADE => [typeof(void)],
			EventType.ON_ELEMENTAL_SPIRIT_LEARN => [typeof(void)],

			// Instance events
			EventType.ON_INSTANCE_CREATED => [typeof(void)],
			EventType.ON_INSTANCE_DESTROY => [typeof(void)],
			EventType.ON_INSTANCE_ENTER => [typeof(void)],
			EventType.ON_INSTANCE_LEAVE => [typeof(void)],
			EventType.ON_INSTANCE_STATUS_CHANGE => [typeof(void)],

			// Server events
			EventType.ON_SERVER_START => [typeof(void)],
			EventType.ON_DAY_NIGHT_CHANGE => [typeof(void)],
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}