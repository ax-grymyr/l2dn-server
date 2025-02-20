using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct WarehouseWithdrawalListPacket: IOutgoingPacket
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(WarehouseWithdrawalListPacket));

    public const int PRIVATE = 1;
    public const int CLAN = 2;
    public const int CASTLE = 3; // not sure
    public const int FREIGHT = 1;

    private readonly int _sendType;
    private readonly long _playerAdena;
    private readonly int _invSize;
    private readonly List<Item> _items;
    private readonly List<int> _itemsStackable; // TODO: not used
    /**
     * <ul>
     * <li>0x01-Private Warehouse</li>
     * <li>0x02-Clan Warehouse</li>
     * <li>0x03-Castle Warehouse</li>
     * <li>0x04-Warehouse</li>
     * </ul>
     */
    private readonly int _whType;

    public WarehouseWithdrawalListPacket(int sendType, Player player, int type)
    {
        _sendType = sendType;
        _whType = type;
        _playerAdena = player.getAdena();
        _invSize = player.getInventory().getSize();
        _items = [];
        _itemsStackable = [];

        ItemContainer? activeWarehouse = player.getActiveWarehouse();
        if (activeWarehouse == null)
        {
            _logger.Error("error while sending withdraw request to: " + player.getName());
            return;
        }

        _items = activeWarehouse.getItems().ToList();
        foreach (Item item in _items)
        {
            if (item.isStackable())
            {
                _itemsStackable.Add(item.getDisplayId());
            }
        }
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.WAREHOUSE_WITHDRAW_LIST);

        writer.WriteByte((byte)_sendType);
        if (_sendType == 2)
        {
            writer.WriteInt16(0);
            writer.WriteInt32(_invSize);
            writer.WriteInt32(_items.Count);
            foreach (Item item in _items)
            {
                InventoryPacketHelper.WriteItem(writer, item);
                writer.WriteInt32(item.ObjectId);
                writer.WriteInt32(0);
                writer.WriteInt32(0);
            }
        }
        else
        {
            writer.WriteInt16((short)_whType);
            writer.WriteInt64(_playerAdena);
            writer.WriteInt32(_invSize);
            writer.WriteInt32(_items.Count);
        }
    }
}