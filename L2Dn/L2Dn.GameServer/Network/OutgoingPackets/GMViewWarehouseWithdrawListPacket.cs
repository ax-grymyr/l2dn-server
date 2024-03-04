using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct GMViewWarehouseWithdrawListPacket: IOutgoingPacket
{
    private readonly int _sendType;
    private readonly ICollection<Item> _items;
    private readonly String _playerName;
    private readonly long _money;
	
    public GMViewWarehouseWithdrawListPacket(int sendType, Player player)
    {
        _sendType = sendType;
        _items = player.getWarehouse().getItems();
        _playerName = player.getName();
        _money = player.getWarehouse().getAdena();
    }
	
    public GMViewWarehouseWithdrawListPacket(int sendType, Clan clan)
    {
        _sendType = sendType;
        _playerName = clan.getLeaderName();
        _items = clan.getWarehouse().getItems();
        _money = clan.getWarehouse().getAdena();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.GM_VIEW_WAREHOUSE_WITHDRAW_LIST);
        
        writer.WriteByte((byte)_sendType);
        if (_sendType == 2)
        {
            writer.WriteInt32(_items.Count);
            writer.WriteInt32(_items.Count);
            foreach (Item item in _items)
            {
                InventoryPacketHelper.WriteItem(writer, item);
                writer.WriteInt32(item.getObjectId());
            }
        }
        else
        {
            writer.WriteString(_playerName);
            writer.WriteInt64(_money);
            writer.WriteInt32(_items.Count);
        }
    }
}