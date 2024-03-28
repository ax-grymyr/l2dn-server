using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExVitalityPointInfoPacket: IOutgoingPacket
{
    private readonly int _vitalityPoints;
	
    public ExVitalityPointInfoPacket(int vitPoints)
    {
        _vitalityPoints = vitPoints;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_VITALITY_POINT_INFO);
        
        writer.WriteInt32(_vitalityPoints);
    }
}