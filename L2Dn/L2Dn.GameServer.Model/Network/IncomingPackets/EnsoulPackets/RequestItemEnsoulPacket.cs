using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Ensoul;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.EnsoulPackets;

// TODO: review and verify this packet for classic
public struct RequestItemEnsoulPacket: IIncomingPacket<GameSession>
{
    private int _itemObjectId;
    private int _type;
    private EnsoulItemOption[]? _options;

    public void ReadContent(PacketBitReader reader)
    {
        _itemObjectId = reader.ReadInt32();
        int options = reader.ReadByte();
        if (options > 0 && options <= 3)
        {
            _options = new EnsoulItemOption[options];
            for (int i = 0; i < options; i++)
            {
                _type = reader.ReadByte(); // 1 = normal ; 2 = mystic
                int position = reader.ReadByte();
                int soulCrystalObjectId = reader.ReadInt32();
                int soulCrystalOption = reader.ReadInt32();
                if (position > 0 && position < 3 && (_type == 1 || _type == 2))
                {
                    _options[i] = new EnsoulItemOption(_type, position, soulCrystalObjectId, soulCrystalOption);
                }
            }
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		if (player.getPrivateStoreType() != PrivateStoreType.NONE)
		{
			player.sendPacket(SystemMessageId.SOUL_CRYSTAL_INSERTION_IS_IMPOSSIBLE_WHEN_PRIVATE_STORE_AND_WORKSHOP_ARE_OPENED);
			return ValueTask.CompletedTask;
		}

		if (player.hasAbnormalType(AbnormalType.FREEZING))
		{
			player.sendPacket(SystemMessageId.SOUL_CRYSTAL_INSERTION_IS_IMPOSSIBLE_WHILE_IN_FROZEN_STATE);
			return ValueTask.CompletedTask;
		}

		if (player.isDead())
		{
			player.sendPacket(SystemMessageId.SOUL_CRYSTAL_INSERTION_IS_IMPOSSIBLE_IF_THE_CHARACTER_IS_DEAD);
			return ValueTask.CompletedTask;
		}

		if (player.getActiveTradeList() != null || player.hasItemRequest())
		{
			player.sendPacket(SystemMessageId.SOUL_CRYSTAL_INSERTION_IS_IMPOSSIBLE_DURING_EXCHANGE);
			return ValueTask.CompletedTask;
		}

		if (player.hasAbnormalType(AbnormalType.PARALYZE))
		{
			player.sendPacket(SystemMessageId.SOUL_CRYSTAL_INSERTION_IS_IMPOSSIBLE_WHILE_THE_CHARACTER_IS_PETRIFIED);
			return ValueTask.CompletedTask;
		}

		if (player.isFishing())
		{
			player.sendPacket(SystemMessageId.SOUL_CRYSTAL_INSERTION_IS_IMPOSSIBLE_DURING_FISHING);
			return ValueTask.CompletedTask;
		}

		if (player.isSitting())
		{
			player.sendPacket(SystemMessageId.SOUL_CRYSTAL_INSERTION_IS_IMPOSSIBLE_WHILE_SITTING);
			return ValueTask.CompletedTask;
		}

		if (AttackStanceTaskManager.getInstance().hasAttackStanceTask(player))
		{
			player.sendPacket(SystemMessageId.SOUL_CRYSTAL_INSERTION_IS_IMPOSSIBLE_DURING_COMBAT);
			return ValueTask.CompletedTask;
		}

		Item? item = player.getInventory().getItemByObjectId(_itemObjectId);
		if (item == null)
		{
			PacketLogger.Instance.Warn("Player: " + player + " attempting to ensoul item without having it!");
			return ValueTask.CompletedTask;
		}

		ItemTemplate template = item.getTemplate();
		CrystalType itemGrade = template.getCrystalType();

		if (_type == 1 && template.getEnsoulSlots() == 0)
		{
			PacketLogger.Instance.Warn("Player: " + player + " attempting to ensoul non ensoulable item: " + item + "!");
			return ValueTask.CompletedTask;
		}

		if (_type == 2 && template.getSpecialEnsoulSlots() == 0)
		{
			PacketLogger.Instance.Warn("Player: " + player + " attempting to special ensoul non special ensoulable item: " + item + "!");
			return ValueTask.CompletedTask;
		}

		if (!item.isEquipable())
		{
			PacketLogger.Instance.Warn("Player: " + player + " attempting to ensoul non equippable item: " + item + "!");
			return ValueTask.CompletedTask;
		}

		if (item.isCommonItem())
		{
			PacketLogger.Instance.Warn("Player: " + player + " attempting to ensoul common item: " + item + "!");
			return ValueTask.CompletedTask;
		}

		if (item.isShadowItem())
		{
			PacketLogger.Instance.Warn("Player: " + player + " attempting to ensoul shadow item: " + item + "!");
			return ValueTask.CompletedTask;
		}

		if (item.isHeroItem())
		{
			PacketLogger.Instance.Warn("Player: " + player + " attempting to ensoul hero item: " + item + "!");
			return ValueTask.CompletedTask;
		}

		if (_options == null || _options.Length == 0)
		{
			PacketLogger.Instance.Warn("Player: " + player + " attempting to ensoul item without any special ability declared!");
			return ValueTask.CompletedTask;
		}

		int success = 0;
		List<ItemInfo> itemsToUpdate = new List<ItemInfo>();
		foreach (EnsoulItemOption itemOption in _options)
		{
			int position = itemOption.getPosition() - 1;
			Item? soulCrystal = player.getInventory().getItemByObjectId(itemOption.getSoulCrystalObjectId());
			if (soulCrystal == null)
			{
				player.sendPacket(SystemMessageId.INVALID_SOUL_CRYSTAL);
				continue;
			}

			EnsoulStone? stone = EnsoulData.getInstance().getStone(soulCrystal.getId());
			if (stone == null)
			{
				continue;
			}

			if (!stone.getOptions().Contains(itemOption.getSoulCrystalOption()))
			{
				PacketLogger.Instance.Warn("Player: " + player + " attempting to ensoul item option that stone doesn't contains!");
				continue;
			}

			EnsoulOption? option = EnsoulData.getInstance().getOption(itemOption.getSoulCrystalOption());
			if (option == null)
			{
				PacketLogger.Instance.Warn("Player: " + player + " attempting to ensoul item option that doesn't exists!");
				continue;
			}

			ItemHolder? fee;
			if (itemOption.getType() == 1)
			{
				// Normal Soul Crystal
				fee = EnsoulData.getInstance().getEnsoulFee(itemGrade, position); // TODO: it was stone.getId() instead of itemGrade
				if ((itemOption.getPosition() == 1 || itemOption.getPosition() == 2) && item.getSpecialAbility(position) != null)
				{
					fee = EnsoulData.getInstance().getResoulFee(itemGrade, position);
				}
			}
			else if (itemOption.getType() == 2)
			{
				// Mystic Soul Crystal
				fee = EnsoulData.getInstance().getEnsoulFee(itemGrade, position + 2); // Client Special type position = 0
				if (itemOption.getPosition() == 1 && item.getAdditionalSpecialAbility(position) != null)
				{
					fee = EnsoulData.getInstance().getResoulFee(itemGrade, position + 2); // Client Special type position = 0
				}
			}
			else
			{
				PacketLogger.Instance.Warn("Player: " + player +
				                           " attempting to ensoul item option with unhandled type: " +
				                           itemOption.getType() + "!");

				continue;
			}

			if (fee == null)
			{
				PacketLogger.Instance.Warn("Player: " + player +
				                           " attempting to ensoul item option that doesn't exists! (unknown fee)");

				continue;
			}

			Item? gemStones = player.getInventory().getItemByItemId(fee.getId());
			if (gemStones == null || gemStones.getCount() < fee.getCount())
			{
				continue;
			}

			if (player.destroyItem("EnsoulOption", soulCrystal, 1, player, true) &&
			    player.destroyItem("EnsoulOption", gemStones, fee.getCount(), player, true))
			{
				item.addSpecialAbility(option, position, stone.getSlotType(), true);
				success = 1;
			}

			if (soulCrystal.isStackable() && soulCrystal.getCount() > 0)
			{
				itemsToUpdate.Add(new ItemInfo(soulCrystal, ItemChangeType.MODIFIED));
			}
			else
			{
				itemsToUpdate.Add(new ItemInfo(soulCrystal, ItemChangeType.REMOVED));
			}
			if (gemStones.isStackable() && gemStones.getCount() > 0)
			{
				itemsToUpdate.Add(new ItemInfo(gemStones, ItemChangeType.MODIFIED));
			}
			else
			{
				itemsToUpdate.Add(new ItemInfo(gemStones, ItemChangeType.REMOVED));
			}

			itemsToUpdate.Add(new ItemInfo(item, ItemChangeType.MODIFIED));
		}

		InventoryUpdatePacket iu = new InventoryUpdatePacket(itemsToUpdate);
		player.sendInventoryUpdate(iu);

		if (item.isEquipped())
		{
			item.applySpecialAbilities();
		}

		player.sendPacket(new ExEnsoulResultPacket(success, item));
		item.updateDatabase(true);

        return ValueTask.CompletedTask;
    }

    private sealed class EnsoulItemOption
    {
        private readonly int _type;
        private readonly int _position;
        private readonly int _soulCrystalObjectId;
        private readonly int _soulCrystalOption;

        public EnsoulItemOption(int type, int position, int soulCrystalObjectId, int soulCrystalOption)
        {
            _type = type;
            _position = position;
            _soulCrystalObjectId = soulCrystalObjectId;
            _soulCrystalOption = soulCrystalOption;
        }

        public int getType()
        {
            return _type;
        }

        public int getPosition()
        {
            return _position;
        }

        public int getSoulCrystalObjectId()
        {
            return _soulCrystalObjectId;
        }

        public int getSoulCrystalOption()
        {
            return _soulCrystalOption;
        }
    }
}