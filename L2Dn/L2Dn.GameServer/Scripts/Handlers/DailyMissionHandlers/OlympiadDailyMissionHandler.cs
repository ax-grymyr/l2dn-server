using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Olympiads;
using L2Dn.GameServer.Model.Events.Listeners;

namespace L2Dn.GameServer.Scripts.Handlers.DailyMissionHandlers;

/**
 * @author UnAfraid
 */
public class OlympiadDailyMissionHandler: AbstractDailyMissionHandler
{
	private readonly int _amount;
	private readonly bool _winOnly;
	
	public OlympiadDailyMissionHandler(DailyMissionDataHolder holder): base(holder)
	{
		_amount = holder.getRequiredCompletions();
		_winOnly = holder.getParams().getBoolean("winOnly", false);
	}
	
	public override void init()
	{
		Containers.Global().addListener(new ConsumerEventListener(this, EventType.ON_OLYMPIAD_MATCH_RESULT,
			@event => onOlympiadMatchResult((OnOlympiadMatchResult)@event), this));
	}
	
	public override bool isAvailable(Player player)
	{
		DailyMissionPlayerEntry entry = getPlayerEntry(player.getObjectId(), false);
		if (entry != null)
		{
			switch (entry.getStatus())
			{
				case DailyMissionStatus.NOT_AVAILABLE: // Initial state
				{
					if (entry.getProgress() >= _amount)
					{
						entry.setStatus(DailyMissionStatus.AVAILABLE);
						storePlayerEntry(entry);
					}
					break;
				}
				case DailyMissionStatus.AVAILABLE:
				{
					return true;
				}
			}
		}
		return false;
	}
	
	private void onOlympiadMatchResult(OnOlympiadMatchResult @event)
	{
		if (@event.getWinner() != null)
		{
			DailyMissionPlayerEntry winnerEntry = getPlayerEntry(@event.getWinner().getObjectId(), true);
			if (winnerEntry.getStatus() == DailyMissionStatus.NOT_AVAILABLE)
			{
				if (winnerEntry.increaseProgress() >= _amount)
				{
					winnerEntry.setStatus(DailyMissionStatus.AVAILABLE);
				}
				storePlayerEntry(winnerEntry);
			}
		}
		
		if (!_winOnly && (@event.getLoser() != null))
		{
			DailyMissionPlayerEntry loseEntry = getPlayerEntry(@event.getLoser().getObjectId(), true);
			if (loseEntry.getStatus() == DailyMissionStatus.NOT_AVAILABLE)
			{
				if (loseEntry.increaseProgress() >= _amount)
				{
					loseEntry.setStatus(DailyMissionStatus.AVAILABLE);
				}
				storePlayerEntry(loseEntry);
			}
		}
	}
}