using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct ManorListPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0xFE); // packet code
        writer.WriteInt16(0x22); // packet ex code (0x1B in C4)

        writer.WriteInt32(0); // count of castles
        // residence id for each castle

        // C4
        // string[] manorNames = { "gludio", "dion", "giran", "oren", "aden", "innadril", "goddard", "rune" };
        // writer.WriteInt32(manorNames.Length);
        // for (int i = 0; i < manorNames.Length; i++)
        // {
        //     writer.WriteInt32(i + 1);
        //     writer.WriteString(manorNames[i]);
        // }
    }
}
