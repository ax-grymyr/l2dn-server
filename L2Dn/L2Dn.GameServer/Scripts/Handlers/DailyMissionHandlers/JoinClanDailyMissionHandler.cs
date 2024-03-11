using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Events.Listeners;

namespace L2Dn.GameServer.Scripts.Handlers.DailyMissionHandlers;

/**
 * @author kamikadzz
 */
public class JoinClanDailyMissionHandler: AbstractDailyMissionHandler
{
	public JoinClanDailyMissionHandler(DailyMissionDataHolder holder): base(holder)
	{
	}
	
	public override bool isAvailable(Player player)
	{
		DailyMissionPlayerEntry? entry = player.getDailyMissions().getEntry(getHolder().getId());
		return (entry != null) && (entry.getStatus() == DailyMissionStatus.AVAILABLE);
	}
	
	public override void init()
	{
		Containers.Global().addListener(new ConsumerEventListener(this, EventType.ON_PLAYER_CLAN_JOIN,
			ev => onPlayerClanJoin((OnPlayerClanJoin)ev), this));

		Containers.Global().addListener(new ConsumerEventListener(this, EventType.ON_PLAYER_CLAN_CREATE,
			ev => onPlayerClanCreate((OnPlayerClanCreate)ev), this));
	}
	
	private void onPlayerClanJoin(OnPlayerClanJoin @event)
	{
		Player player = @event.getClanMember().getPlayer();
		DailyMissionPlayerEntry missionData = player.getDailyMissions().getOrCreateEntry(getHolder().getId());
		processMission(player, missionData);
	}
	
	private void onPlayerClanCreate(OnPlayerClanCreate @event)
	{
		Player player = @event.getPlayer();
		DailyMissionPlayerEntry missionData = player.getDailyMissions().getOrCreateEntry(getHolder().getId());
		processMission(player, missionData);
	}
	
	private void processMission(Player player, DailyMissionPlayerEntry missionData)
	{
		if (missionData.getProgress() == 1)
		{
			missionData.setStatus(DailyMissionStatus.COMPLETED);
		}
		else
		{
			missionData.setProgress(1);
			missionData.setStatus(DailyMissionStatus.AVAILABLE);
		}
		
		player.getDailyMissions().storeEntry(missionData);
	}
}