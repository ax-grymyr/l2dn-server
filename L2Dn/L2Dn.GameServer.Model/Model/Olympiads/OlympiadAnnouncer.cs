using L2Dn.GameServer.Data;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Olympiads;

/**
 * @author DS
 */
public class OlympiadAnnouncer: Runnable
{
	private const int OLY_MANAGER = 31688;

	private int _currentStadium = 0;

	public void run()
	{
		OlympiadGameTask task;
		for (int i = OlympiadGameManager.getInstance().getNumberOfStadiums(); --i >= 0; _currentStadium++)
		{
			if (_currentStadium >= OlympiadGameManager.getInstance().getNumberOfStadiums())
			{
				_currentStadium = 0;
			}

			task = OlympiadGameManager.getInstance().getOlympiadTask(_currentStadium);
			if (task != null && task.getGame() != null && task.needAnnounce())
			{
				NpcStringId npcString;
				string arenaId = (task.getGame().getStadiumId() + 1).ToString();
				switch (task.getGame().getType())
				{
					case CompetitionType.NON_CLASSED:
					{
						npcString = NpcStringId.THE_DUELS_BETWEEN_PLAYERS_OF_ANY_CLASS_WILL_START_SHORTLY_IN_ARENA_S1;
						break;
					}
					case CompetitionType.CLASSED:
					{
						npcString = NpcStringId.THE_CLASS_DUELS_WILL_START_SHORTLY_IN_ARENA_S1;
						break;
					}
					default:
					{
						continue;
					}
				}

				foreach (Spawn spawn in SpawnTable.getInstance().getSpawns(OLY_MANAGER))
				{
					Npc manager = spawn.getLastSpawn();
					if (manager != null)
					{
						manager.broadcastSay(ChatType.NPC_SHOUT, npcString, arenaId);
					}
				}

				break;
			}
		}
	}
}