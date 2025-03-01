using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Olympiads;
using L2Dn.GameServer.Model.Olympiads;

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
		GlobalEvents.Global.Subscribe<OnOlympiadMatchResult>(this, onOlympiadMatchResult);
	}

	public override bool isAvailable(Player player)
	{
		DailyMissionPlayerEntry? entry = player.getDailyMissions().getEntry(getHolder().getId());
		if (entry != null)
		{
			switch (entry.getStatus())
			{
				case DailyMissionStatus.NOT_AVAILABLE: // Initial state
				{
					if (entry.getProgress() >= _amount)
					{
						entry.setStatus(DailyMissionStatus.AVAILABLE);
						player.getDailyMissions().storeEntry(entry);
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
        Participant? winner = @event.getWinner();
		if (winner != null)
		{
			Player player = winner.getPlayer();
			DailyMissionPlayerEntry winnerEntry = player.getDailyMissions().getOrCreateEntry(getHolder().getId());
			if (winnerEntry.getStatus() == DailyMissionStatus.NOT_AVAILABLE)
			{
				if (winnerEntry.increaseProgress() >= _amount)
				{
					winnerEntry.setStatus(DailyMissionStatus.AVAILABLE);
				}

				player.getDailyMissions().storeEntry(winnerEntry);
			}
		}

		if (!_winOnly && @event.getLoser() != null)
		{
			Player player = @event.getLoser().getPlayer();
			DailyMissionPlayerEntry loseEntry = player.getDailyMissions().getOrCreateEntry(getHolder().getId());
			if (loseEntry.getStatus() == DailyMissionStatus.NOT_AVAILABLE)
			{
				if (loseEntry.increaseProgress() >= _amount)
				{
					loseEntry.setStatus(DailyMissionStatus.AVAILABLE);
				}

				player.getDailyMissions().storeEntry(loseEntry);
			}
		}
	}
}