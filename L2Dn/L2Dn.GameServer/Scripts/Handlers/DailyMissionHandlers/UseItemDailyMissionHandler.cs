using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Items;
using L2Dn.GameServer.Model.Events.Listeners;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.DailyMissionHandlers;

/**
 * @author CostyKiller
 */
public class UseItemDailyMissionHandler: AbstractDailyMissionHandler
{
	private readonly int _amount;
	private readonly int _minLevel;
	private readonly int _maxLevel;
	private readonly Set<int> _itemIds = new();
	
	public UseItemDailyMissionHandler(DailyMissionDataHolder holder): base(holder)
	{
		_amount = holder.getRequiredCompletions();
		_minLevel = holder.getParams().getInt("minLevel", 0);
		_maxLevel = holder.getParams().getInt("maxLevel", int.MaxValue);
		String itemIds = holder.getParams().getString("itemIds", "");
		if (!itemIds.isEmpty())
		{
			foreach (String s in itemIds.Split(","))
			{
				int id = int.Parse(s);
				if (!_itemIds.Contains(id))
				{
					_itemIds.add(id);
				}
			}
		}
	}
	
	public override void init()
	{
		Containers.Global().addListener(new ConsumerEventListener(Containers.Global(), EventType.ON_ITEM_USE,
			@event => onItemUse((OnItemUse)@event), this));
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
	
	private void onItemUse(OnItemUse @event)
	{
		Player player = @event.getPlayer();
		if (_minLevel > 0)
		{
			if ((player.getLevel() < _minLevel) || (player.getLevel() > _maxLevel) || _itemIds.isEmpty())
			{
				return;
			}
			if (_itemIds.Contains(@event.getItem().getId()))
			{
				processPlayerProgress(player);
			}
		}
	}
	
	private void processPlayerProgress(Player player)
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