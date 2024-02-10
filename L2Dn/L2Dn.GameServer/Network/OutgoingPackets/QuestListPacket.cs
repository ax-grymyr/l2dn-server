using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct QuestListPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x80); // packet code

        // C4
        // writer.WriteInt32(0); // quest count

        writer.WriteInt16(0); // quest count
        // for each quest: quest id, flags
        writer.WriteZeros(128); // oneTimeQuestMask ???
    }
}
