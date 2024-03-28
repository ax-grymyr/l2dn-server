using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Quests;

public readonly struct ExTeleportUiPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_TELEPORT_UI);
        
        writer.WriteInt32(1); // PriceRatio
    }
}