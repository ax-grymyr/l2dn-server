using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Events.Listeners;

namespace L2Dn.GameServer.Scripts.Handlers.DailyMissionHandlers;

/**
 * @author UnAfraid
 */
public class QuestDailyMissionHandler: AbstractDailyMissionHandler
{
	private readonly int _amount;
	
	public QuestDailyMissionHandler(DailyMissionDataHolder holder): base(holder)
	{
		_amount = holder.getRequiredCompletions();
	}
	
	public override void init()
	{
		Containers.Players().addListener(new ConsumerEventListener(this, EventType.ON_PLAYER_QUEST_COMPLETE,
			@event => onQuestComplete((OnPlayerQuestComplete)@event), this));
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
	
	private void onQuestComplete(OnPlayerQuestComplete @event)
	{
		Player player = @event.getPlayer();
		if (@event.getQuestType() == QuestType.DAILY)
		{
			DailyMissionPlayerEntry entry = getPlayerEntry(player.getObjectId(), true);
			if (entry.getStatus() == DailyMissionStatus.NOT_AVAILABLE)
			{
				if (entry.increaseProgress() >= _amount)
				{
					entry.setStatus(DailyMissionStatus.AVAILABLE);
				}
				storePlayerEntry(entry);
			}
		}
	}
}