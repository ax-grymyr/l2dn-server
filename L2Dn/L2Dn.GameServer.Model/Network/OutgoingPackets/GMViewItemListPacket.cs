using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct GMViewItemListPacket: IOutgoingPacket
{
    private readonly int _sendType;
    private readonly List<Item> _items;
    private readonly int _limit;
    private readonly String _playerName;
	
    public GMViewItemListPacket(int sendType, Player player)
    {
        _sendType = sendType;
        _playerName = player.getName();
        _limit = player.getInventoryLimit();
        _items = new List<Item>();
        foreach (Item item in player.getInventory().getItems())
        {
            _items.Add(item);
        }
    }
	
    public GMViewItemListPacket(int sendType, Pet cha)
    {
        _sendType = sendType;
        _playerName = cha.getName();
        _limit = cha.getInventoryLimit();
        _items = new List<Item>();
        foreach (Item item in cha.getInventory().getItems())
        {
            _items.Add(item);
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.GM_VIEW_ITEM_LIST);
        
        writer.WriteByte((byte)_sendType);
        if (_sendType == 2)
        {
            writer.WriteInt32(_items.Count);
        }
        else
        {
            writer.WriteString(_playerName);
            writer.WriteInt32(_limit); // inventory limit
        }
        writer.WriteInt32(_items.Count);
        foreach (Item item in _items)
        {
            InventoryPacketHelper.WriteItem(writer, item);
        }
    }
}