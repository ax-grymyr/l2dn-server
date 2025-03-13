using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestProcureCropListPacket: IIncomingPacket<GameSession>
{
    private const int BATCH_LENGTH = 20; // length of the one item

    private List<CropHolder>? _items;

    public void ReadContent(PacketBitReader reader)
    {
        int count = reader.ReadInt32();
        if (count <= 0 || count > Config.MAX_ITEM_IN_PACKET || count * BATCH_LENGTH != reader.Length)
        {
            return;
        }

        _items = new List<CropHolder>(count);
        for (int i = 0; i < count; i++)
        {
            int objId = reader.ReadInt32();
            int itemId = reader.ReadInt32();
            int manorId = reader.ReadInt32();
            long cnt = reader.ReadInt64();
            if (objId < 1 || itemId < 1 || manorId < 0 || cnt < 0)
            {
                _items = null;
                return;
            }

            _items.Add(new CropHolder(objId, itemId, cnt, manorId));
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		if (_items == null)
			return ValueTask.CompletedTask;

		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		CastleManorManager manor = CastleManorManager.getInstance();
		if (manor.isUnderMaintenance())
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		Npc? manager = player.getLastFolkNPC();
        Castle? castle = manager?.getCastle();
		if (!(manager is Merchant) || !manager.canInteract(player) || castle == null)
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		int castleId = castle.getResidenceId();
		if (manager.getParameters().getInt("manor_id", -1) != castleId)
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		long slots = 0;
		long weight = 0;
		foreach (CropHolder i in _items)
		{
			Item? item = player.getInventory().getItemByObjectId(i.ObjectId);
			if (item == null || item.getCount() < i.getCount() || item.getId() != i.getId())
			{
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}

			CropProcure? cp = i.getCropProcure();
			if (cp == null || cp.getAmount() < i.getCount())
			{
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}

			ItemTemplate? template = ItemData.getInstance().getTemplate(i.getRewardId());
            if (template == null)
            {
                player.sendPacket(ActionFailedPacket.STATIC_PACKET);
                return ValueTask.CompletedTask;
            }

			weight += i.getCount() * template.getWeight();
			if (!template.isStackable())
			{
				slots += i.getCount();
			}
			else if (player.getInventory().getItemByItemId(i.getRewardId()) == null)
			{
				slots++;
			}
		}

		if (!player.getInventory().validateWeight(weight))
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_WEIGHT_LIMIT);
			return ValueTask.CompletedTask;
		}

		if (!player.getInventory().validateCapacity(slots))
		{
			player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL);
			return ValueTask.CompletedTask;
		}

		// Used when Config.ALT_MANOR_SAVE_ALL_ACTIONS == true
		int updateListSize = Config.ALT_MANOR_SAVE_ALL_ACTIONS ? _items.Count : 0;
		List<CropProcure> updateList = new(updateListSize);

		// Proceed the purchase
		foreach (CropHolder i in _items)
        {
            ItemTemplate? rewardItemTemplate = ItemData.getInstance().getTemplate(i.getRewardId());
            if (rewardItemTemplate == null)
                continue;

			long rewardPrice = rewardItemTemplate.getReferencePrice();
			if (rewardPrice == 0)
				continue;

			long rewardItemCount = i.getPrice() / rewardPrice;
			if (rewardItemCount < 1)
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.FAILED_IN_TRADING_S2_OF_S1_CROPS);
				sm.Params.addItemName(i.getId());
				sm.Params.addLong(i.getCount());
				player.sendPacket(sm);
				continue;
			}

			// Fee for selling to other manors
			long fee = castleId == i.getManorId() ? 0 : (long) (i.getPrice() * 0.05);
			if (fee != 0 && player.getAdena() < fee)
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.FAILED_IN_TRADING_S2_OF_S1_CROPS);
				sm.Params.addItemName(i.getId());
				sm.Params.addLong(i.getCount());
				player.sendPacket(sm);

				sm = new SystemMessagePacket(SystemMessageId.NOT_ENOUGH_ADENA);
				player.sendPacket(sm);
				continue;
			}

			CropProcure cp = i.getCropProcure()!;
			if (!cp.decreaseAmount(i.getCount()) || (fee > 0 && !player.reduceAdena("Manor", fee, manager, true)) || !player.destroyItem("Manor", i.ObjectId, i.getCount(), manager, true))
			{
				continue;
			}
			player.addItem("Manor", i.getRewardId(), rewardItemCount, manager, true);
			if (Config.ALT_MANOR_SAVE_ALL_ACTIONS)
			{
				updateList.Add(cp);
			}
		}

		if (Config.ALT_MANOR_SAVE_ALL_ACTIONS)
		{
			manor.updateCurrentProcure(castleId, updateList);
		}

		return ValueTask.CompletedTask;
	}

	private sealed class CropHolder(int objectId, int id, long count, int manorId)
        : UniqueItemHolder(id, objectId, count)
    {
        private CropProcure? _cp;
		private int _rewardId;

        public int getManorId()
		{
			return manorId;
		}

		public long getPrice()
		{
			return getCount() * (_cp?.getPrice() ?? 0);
		}

		public CropProcure? getCropProcure()
        {
            return _cp ??= CastleManorManager.getInstance().getCropProcure(manorId, getId(), false);
        }

		public int getRewardId()
        {
			if (_rewardId == 0 && _cp != null)
				_rewardId = CastleManorManager.getInstance().getSeedByCrop(_cp.getId())?.getReward(_cp.getReward()) ?? 0;

            return _rewardId;
		}
	}
}