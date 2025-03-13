using L2Dn.Events;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Events.Impl.Items;
using L2Dn.GameServer.Model.Items.Instances;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.ItemContainers;

public class ClanWarehouse: Warehouse
{
	private readonly Clan _clan;

	public ClanWarehouse(Clan clan)
	{
		_clan = clan;
	}

	public override string getName()
	{
		return "ClanWarehouse";
	}

	public override int getOwnerId()
	{
		return _clan.getId();
	}

	public override Player getOwner()
	{
		return _clan.getLeader().getPlayer() ?? throw new InvalidOperationException("Clan leader is null");
	}

	public override ItemLocation getBaseLocation()
	{
		return ItemLocation.CLANWH;
	}

	public override bool validateCapacity(long slots)
	{
		return _items.size() + slots <= Config.Character.WAREHOUSE_SLOTS_CLAN;
	}

	public override Item? addItem(string process, int itemId, long count, Player? actor, object? reference)
	{
		Item? item = base.addItem(process, itemId, count, actor, reference);

		// Notify to scripts
        if (item != null)
        {
            EventContainer itemEvents = item.getTemplate().Events;
            if (itemEvents.HasSubscribers<OnClanWhItemAdd>())
            {
                itemEvents.NotifyAsync(new OnClanWhItemAdd(process, actor, item, this));
            }
        }

        return item;
	}

	public override Item? addItem(string process, Item item, Player? actor, object? reference)
	{
		// Notify to scripts
		EventContainer itemEvents = item.getTemplate().Events;
		if (itemEvents.HasSubscribers<OnClanWhItemAdd>())
		{
			itemEvents.NotifyAsync(new OnClanWhItemAdd(process, actor, item, this));
		}

		return base.addItem(process, item, actor, reference);
	}

	public override Item? destroyItem(string? process, Item item, long count, Player? actor, object? reference)
	{
		// Notify to scripts
		EventContainer itemEvents = item.getTemplate().Events;
		if (itemEvents.HasSubscribers<OnClanWhItemDestroy>())
		{
			itemEvents.NotifyAsync(new OnClanWhItemDestroy(process, actor, item, count, this));
		}

		return base.destroyItem(process, item, count, actor, reference);
	}

	public override Item? transferItem(string process, int objectId, long count, ItemContainer target, Player? actor,
		object? reference)
	{
		Item? item = getItemByObjectId(objectId);
        if (item == null)
            return null;

		// Notify to scripts
		EventContainer itemEvents = item.getTemplate().Events;
		if (itemEvents.HasSubscribers<OnClanWhItemTransfer>())
		{
			itemEvents.NotifyAsync(new OnClanWhItemTransfer(process, actor, item, count, target));
		}

		return base.transferItem(process, objectId, count, target, actor, reference);
	}
}