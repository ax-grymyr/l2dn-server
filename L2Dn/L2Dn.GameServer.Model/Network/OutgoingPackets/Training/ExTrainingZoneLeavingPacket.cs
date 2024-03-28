using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Training;

public readonly struct ExTrainingZoneLeavingPacket: IOutgoingPacket
{
    public static readonly ExTrainingZoneLeavingPacket STATIC_PACKET = new();
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_TRAINING_ZONE_LEAVING);
    }
}