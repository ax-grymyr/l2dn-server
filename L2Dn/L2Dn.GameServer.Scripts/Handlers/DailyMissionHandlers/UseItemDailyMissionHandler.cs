using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Items;
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
		string itemIds = holder.getParams().getString("itemIds", "");
		if (!string.IsNullOrEmpty(itemIds))
		{
			foreach (string s in itemIds.Split(","))
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
		GlobalEvents.Global.Subscribe<OnItemUse>(this, onItemUse);
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

	private void onItemUse(OnItemUse @event)
	{
		Player player = @event.getPlayer();
		if (_minLevel > 0)
		{
			if (player.getLevel() < _minLevel || player.getLevel() > _maxLevel || _itemIds.isEmpty())
			{
				return;
			}
			if (_itemIds.Contains(@event.getItem().Id))
			{
				processPlayerProgress(player);
			}
		}
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