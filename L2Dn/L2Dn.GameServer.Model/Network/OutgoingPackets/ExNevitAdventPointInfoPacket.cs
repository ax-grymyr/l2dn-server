using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExNevitAdventPointInfoPacket: IOutgoingPacket
{
    private readonly int _points;
	
    public ExNevitAdventPointInfoPacket(int points)
    {
        _points = points;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BR_AGATHION_ENERGY_INFO);
        
        writer.WriteInt32(_points); // 72 = 1%, max 7200 = 100%
    }
}