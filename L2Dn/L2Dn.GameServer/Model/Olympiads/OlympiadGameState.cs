namespace L2Dn.GameServer.Model.Olympiads;

public enum OlympiadGameState
{
	BEGIN,
	TELEPORT_TO_ARENA,
	GAME_STARTED,
	BATTLE_COUNTDOWN_FIRST,
	BATTLE_COUNTDOWN_SECOND,
	BATTLE_STARTED,
	BATTLE_IN_PROGRESS,
	GAME_STOPPED,
	TELEPORT_TO_TOWN,
	CLEANUP,
	IDLE
}