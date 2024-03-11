using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Items;
using L2Dn.GameServer.Model.Events.Listeners;

namespace L2Dn.GameServer.Scripts.Handlers.DailyMissionHandlers;

/**
 * @author CostyKiller
 */
public class PurgeRewardDailyMissionHandler: AbstractDailyMissionHandler
{
	private readonly int _amount;
	private readonly int _minLevel;
	private readonly int _maxLevel;
	
	public PurgeRewardDailyMissionHandler(DailyMissionDataHolder holder): base(holder)
	{
		_amount = holder.getRequiredCompletions();
		_minLevel = holder.getParams().getInt("minLevel", 0);
		_maxLevel = holder.getParams().getInt("maxLevel", int.MaxValue);
	}
	
	public override void init()
	{
		Containers.Global().addListener(new ConsumerEventListener(Containers.Global(), EventType.ON_ITEM_PURGE_REWARD,
			@event => onItemPurgeReward((OnItemPurgeReward)@event), this));
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
	
	private void onItemPurgeReward(OnItemPurgeReward @event)
	{
		Player player = @event.getPlayer();
		if ((player.getLevel() < _minLevel) || (player.getLevel() > _maxLevel))
		{
			return;
		}
		processPlayerProgress(player);
	}
	
	private void processPlayerProgress(Player player)
	{
		DailyMissionPlayerEntry entry = player.getDailyMissions().getOrCreateEntry(getHolder().getId());
		if (entry.getStatus() == DailyMissionStatus.NOT_AVAILABLE)
		{
			if (entry.increaseProgress() >= _amount)
			{
				entry.setStatus(DailyMissionStatus.AVAILABLE);
			}

			player.getDailyMissions().storeEntry(entry);
		}
	}
}