using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Players;

namespace L2Dn.GameServer.Scripts.Handlers.DailyMissionHandlers;

/**
 * @author Iris, Mobius
 */
public class LoginWeekendDailyMissionHandler: AbstractDailyMissionHandler
{
	public LoginWeekendDailyMissionHandler(DailyMissionDataHolder holder): base(holder)
	{
	}
	
	public override bool isAvailable(Player player)
	{
		DailyMissionPlayerEntry? entry = player.getDailyMissions().getEntry(getHolder().getId());
		return entry != null && entry.getStatus() == DailyMissionStatus.AVAILABLE;
	}
	
	public override void init()
	{
		GlobalEvents.Global.Subscribe<OnPlayerLogin>(this, onPlayerLogin);
	}
	
	private void onPlayerLogin(OnPlayerLogin @event)
	{
		Player player = @event.getPlayer();
		DailyMissionPlayerEntry entry = player.getDailyMissions().getOrCreateEntry(getHolder().getId());
		if (entry.getStatus() != DailyMissionStatus.COMPLETED)
		{
			DateTime now = DateTime.Now;
			DayOfWeek currentDay = now.DayOfWeek;
			if ((currentDay == DayOfWeek.Saturday || currentDay == DayOfWeek.Sunday || currentDay == DayOfWeek.Monday) //
				&& now.Hour < 6 && now.Minute < 30)
			{
				entry.setProgress(1);
				entry.setStatus(DailyMissionStatus.AVAILABLE);
			}
		}
		
		player.getDailyMissions().storeEntry(entry);
	}
}