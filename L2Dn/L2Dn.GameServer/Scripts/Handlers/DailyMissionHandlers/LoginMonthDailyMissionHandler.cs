using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Events.Listeners;

namespace L2Dn.GameServer.Handlers.DailyMissionHandlers;

/**
 * @author Iris, Mobius
 */
public class LoginMonthDailyMissionHandler: AbstractDailyMissionHandler
{
	public LoginMonthDailyMissionHandler(DailyMissionDataHolder holder): base(holder)
	{
	}
	
	public override bool isAvailable(Player player)
	{
		DailyMissionPlayerEntry entry = getPlayerEntry(player.getObjectId(), false);
		return (entry != null) && (entry.getStatus() == DailyMissionStatus.AVAILABLE);
	}
	
	public override void init()
	{
		Containers.Global().addListener(new ConsumerEventListener(this, EventType.ON_PLAYER_LOGIN,
			@event => onPlayerLogin((OnPlayerLogin)@event), this));
	}
	
	private void onPlayerLogin(OnPlayerLogin @event)
	{
		DailyMissionPlayerEntry entry = getPlayerEntry(@event.getPlayer().getObjectId(), true);
		if (entry.getStatus() != DailyMissionStatus.COMPLETED)
		{
			entry.setProgress(1);
			entry.setStatus(DailyMissionStatus.AVAILABLE);
		}
		storePlayerEntry(entry);
	}
}