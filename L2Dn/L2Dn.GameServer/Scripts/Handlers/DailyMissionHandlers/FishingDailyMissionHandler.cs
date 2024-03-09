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
public class FishingDailyMissionHandler: AbstractDailyMissionHandler
{
	private readonly int _amount;
	private readonly int _minLevel;
	private readonly int _maxLevel;
	
	public FishingDailyMissionHandler(DailyMissionDataHolder holder): base(holder)
	{
		_amount = holder.getRequiredCompletions();
		_minLevel = holder.getParams().getInt("minLevel", 0);
		_maxLevel = holder.getParams().getInt("maxLevel", int.MaxValue);
	}
	
	public override void init()
	{
		Containers.Global().addListener(new ConsumerEventListener(Containers.Global(), EventType.ON_PLAYER_FISHING,
			@event => onPlayerFishing((OnPlayerFishing)@event), this));
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
	
	private void onPlayerFishing(OnPlayerFishing @event)
	{
		Player player = @event.getPlayer();
		if ((player.getLevel() < _minLevel) || (player.getLevel() > _maxLevel))
		{
			return;
		}
		
		if (@event.getReason() == FishingEndReason.WIN)
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