using L2Dn.Events;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Events.Impl.Items;
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
		EventContainer itemEvents = item.getTemplate().Events;
		if (itemEvents.HasSubscribers<OnClanWhItemAdd>())
		{
			itemEvents.NotifyAsync(new OnClanWhItemAdd(process, actor, item, this));
		}

		return item;
	}

	public override Item addItem(String process, Item item, Player actor, Object reference)
	{
		// Notify to scripts
		EventContainer itemEvents = item.getTemplate().Events;
		if (itemEvents.HasSubscribers<OnClanWhItemAdd>())
		{
			itemEvents.NotifyAsync(new OnClanWhItemAdd(process, actor, item, this));
		}

		return base.addItem(process, item, actor, reference);
	}

	public override Item destroyItem(String process, Item item, long count, Player actor, Object reference)
	{
		// Notify to scripts
		EventContainer itemEvents = item.getTemplate().Events;
		if (itemEvents.HasSubscribers<OnClanWhItemDestroy>())
		{
			itemEvents.NotifyAsync(new OnClanWhItemDestroy(process, actor, item, count, this));
		}

		return base.destroyItem(process, item, count, actor, reference);
	}

	public override Item transferItem(String process, int objectId, long count, ItemContainer target, Player actor,
		Object reference)
	{
		Item item = getItemByObjectId(objectId);

		// Notify to scripts
		EventContainer itemEvents = item.getTemplate().Events;
		if (itemEvents.HasSubscribers<OnClanWhItemTransfer>())
		{
			itemEvents.NotifyAsync(new OnClanWhItemTransfer(process, actor, item, count, target));
		}

		return base.transferItem(process, objectId, count, target, actor, reference);
	}
}