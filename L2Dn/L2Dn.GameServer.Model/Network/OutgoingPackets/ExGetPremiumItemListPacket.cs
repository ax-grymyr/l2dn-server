using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExGetPremiumItemListPacket: IOutgoingPacket
{
    private readonly Map<int, PremiumItem> _map;
	
    public ExGetPremiumItemListPacket(Player player)
    {
        _map = player.getPremiumItemList();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_GET_PREMIUM_ITEM_LIST);
        
        writer.WriteInt32(_map.Count);
        foreach (var entry in _map)
        {
            PremiumItem item = entry.Value;
            writer.WriteInt64(entry.Key);
            writer.WriteInt32(item.getItemId());
            writer.WriteInt64(item.getCount());
            writer.WriteInt32(0); // ?
            writer.WriteString(item.getSender());
        }
    }
}