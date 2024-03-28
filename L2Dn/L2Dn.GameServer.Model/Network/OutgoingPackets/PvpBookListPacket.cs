using L2Dn.Extensions;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PvpBookListPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PVPBOOK_LIST);
        
        int size = 1;
        writer.WriteInt32(4); // show killer's location count
        writer.WriteInt32(5); // teleport count
        writer.WriteInt32(size); // killer count
        for (int i = 0; i < size; i++)
        {
            writer.WriteSizedString("killer" + i); // killer name
            writer.WriteSizedString("clanKiller" + i); // killer clan name
            writer.WriteInt32(15); // killer level
            writer.WriteInt32(2); // killer race
            writer.WriteInt32(10); // killer class
            writer.WriteInt32(DateTime.UtcNow.getEpochSecond()); // kill time
            writer.WriteByte(1); // is online
        }
    }
}