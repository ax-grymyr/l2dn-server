using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct WarehouseDepositListPacket: IOutgoingPacket
{
    public const int PRIVATE = 1; // TODO enum
    public const int CLAN = 2;
    public const int CASTLE = 3;
    public const int FREIGHT = 1;
	
    private readonly  int _sendType;
    private readonly  long _playerAdena;
    private readonly  List<Item> _items;
    private readonly  List<int> _itemsStackable;
    /**
     * <ul>
     * <li>0x01-Private Warehouse</li>
     * <li>0x02-Clan Warehouse</li>
     * <li>0x03-Castle Warehouse</li>
     * <li>0x04-Warehouse</li>
     * </ul>
     */
    private readonly int _whType;
	
    public WarehouseDepositListPacket(int sendType, Player player, int type)
    {
        _sendType = sendType;
        _whType = type;
        _playerAdena = player.getAdena();
        _items = new List<Item>();
        _itemsStackable = new List<int>();
        bool isPrivate = _whType == PRIVATE;
        foreach (Item temp in player.getInventory().getAvailableItems(true, isPrivate, false))
        {
            if (temp != null && temp.isDepositable(isPrivate))
            {
                _items.Add(temp);
            }
            if (temp != null && temp.isDepositable(isPrivate) && temp.isStackable())
            {
                _itemsStackable.Add(temp.getDisplayId());
            }
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.WAREHOUSE_DEPOSIT_LIST);
        
        writer.WriteByte((byte)_sendType);
        if (_sendType == 2)
        {
            writer.WriteInt32(_whType);
            writer.WriteInt32(_items.Count);
            foreach (Item item in _items)
            {
                InventoryPacketHelper.WriteItem(writer, item);
                writer.WriteInt32(item.ObjectId);
            }
        }
        else
        {
            writer.WriteInt16((short)_whType);
            writer.WriteInt64(_playerAdena);
            writer.WriteInt32(_itemsStackable.Count);
            writer.WriteInt32(_items.Count);
        }
    }
}