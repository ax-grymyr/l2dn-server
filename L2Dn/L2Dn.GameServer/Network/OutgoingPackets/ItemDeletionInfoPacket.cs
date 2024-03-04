using L2Dn.Extensions;
using L2Dn.GameServer.InstanceManagers.Events;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ItemDeletionInfoPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ITEM_DELETION_INFO);
		
        // Items.
        Map<int, DateTime> itemDates = ItemDeletionInfoManager.getInstance().getItemDates();
        writer.WriteInt32(itemDates.size());
        foreach (var info in itemDates)
        {
            writer.WriteInt32(info.Key); // item
            writer.WriteInt32(info.Value.getEpochSecond()); // date
        }
		
        // Skills.
        writer.WriteInt32(0);
    }
}