using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.ItemContainers;

public class ClanWarehouse: Warehouse
{
	private readonly Clan _clan;

	public ClanWarehouse(Clan clan)
	{
		_clan = clan;
	}

	public override String getName()
	{
		return "ClanWarehouse";
	}

	public override int getOwnerId()
	{
		return _clan.getId();
	}

	public override Player getOwner()
	{
		return _clan.getLeader().getPlayer();
	}

	public override ItemLocation getBaseLocation()
	{
		return ItemLocation.CLANWH;
	}

	public override bool validateCapacity(long slots)
	{
		return (_items.size() + slots) <= Config.WAREHOUSE_SLOTS_CLAN;
	}

	public override Item addItem(String process, int itemId, long count, Player actor, Object reference)
	{
		Item item = base.addItem(process, itemId, count, actor, reference);

		// Notify to scripts
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_CLAN_WH_ITEM_ADD, item.getTemplate()))
		{
			EventDispatcher.getInstance()
				.notifyEventAsync(new OnPlayerClanWHItemAdd(process, actor, item, this), item.getTemplate());
		}

		return item;
	}

	public override Item addItem(String process, Item item, Player actor, Object reference)
	{
		// Notify to scripts
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_CLAN_WH_ITEM_ADD, item.getTemplate()))
		{
			EventDispatcher.getInstance()
				.notifyEventAsync(new OnPlayerClanWHItemAdd(process, actor, item, this), item.getTemplate());
		}

		return base.addItem(process, item, actor, reference);
	}

	public override Item destroyItem(String process, Item item, long count, Player actor, Object reference)
	{
		// Notify to scripts
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_CLAN_WH_ITEM_DESTROY, item.getTemplate()))
		{
			EventDispatcher.getInstance()
				.notifyEventAsync(new OnPlayerClanWHItemDestroy(process, actor, item, count, this), item.getTemplate());
		}

		return base.destroyItem(process, item, count, actor, reference);
	}

	public override Item transferItem(String process, int objectId, long count, ItemContainer target, Player actor,
		Object reference)
	{
		Item item = getItemByObjectId(objectId);

		// Notify to scripts
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_CLAN_WH_ITEM_TRANSFER, item.getTemplate()))
		{
			EventDispatcher.getInstance()
				.notifyEventAsync(new OnPlayerClanWHItemTransfer(process, actor, item, count, target),
					item.getTemplate());
		}

		return base.transferItem(process, objectId, count, target, actor, reference);
	}
}