using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Teleports;

public readonly struct ExShowTeleportUiPacket: IOutgoingPacket
{
    public static readonly ExShowTeleportUiPacket STATIC_PACKET = new();
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_TELEPORT_UI);
    }
}