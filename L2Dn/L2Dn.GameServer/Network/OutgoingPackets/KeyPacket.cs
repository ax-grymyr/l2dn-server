using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

/// <summary>
/// 0x00 - KeyPacket
/// </summary>
internal readonly struct KeyPacket(bool isProtocolOk, int serverId, byte[]? encryptionKey): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        // 0x00 in C4
        writer.WriteByte(0x2E); // packet code

        writer.WriteByte(isProtocolOk);
        
        if (encryptionKey is not null)
        {
            writer.WriteBytes(encryptionKey); // XOR key - 8 bytes
            writer.WriteInt32(1); // use blowfish encryption (missing in C4)
        }
        else
        {
            writer.WriteZeros(8); // XOR key - 8 bytes
            writer.WriteInt32(0); // use blowfish encryption (missing in C4)
        }

        writer.WriteInt32(serverId); // server id
        writer.WriteByte(1); // unknown
        writer.WriteInt32(0); // obfuscation key
        writer.WriteByte(1); // 0 - main version, 1 - classic, 4 - essence
    }
}
