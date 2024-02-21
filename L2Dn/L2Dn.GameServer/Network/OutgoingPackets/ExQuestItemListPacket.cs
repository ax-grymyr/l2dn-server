using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExQuestItemListPacket: IOutgoingPacket
{
    private readonly int _sendType;
    private readonly Player _player;
    private readonly List<Item> _items;
	
    public ExQuestItemListPacket(int sendType, Player player)
    {
        _sendType = sendType;
        _player = player;
        _items = new List<Item>();
        foreach (Item item in player.getInventory().getItems())
        {
            if (item.isQuestItem())
            {
                _items.Add(item);
            }
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_QUEST_ITEM_LIST);
        
        writer.WriteByte((byte)_sendType);
        if (_sendType == 2)
        {
            writer.WriteInt32(_items.Count);
        }
        else
        {
            writer.WriteInt16(0);
        }
        
        writer.WriteInt32(_items.Count);
        foreach (Item item in _items)
        {
            InventoryPacketHelper.WriteItem(writer, item);
        }
        
        InventoryPacketHelper.WriteInventoryBlock(writer, _player.getInventory());
    }
}