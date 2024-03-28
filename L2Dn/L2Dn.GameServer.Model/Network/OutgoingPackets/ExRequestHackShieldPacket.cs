using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExRequestHackShieldPacket: IOutgoingPacket
{
    public static readonly ExRequestHackShieldPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_REQUEST_HACK_SHIELD);
    }
}