using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExRotationPacket: IOutgoingPacket
{
    private readonly int _charId;
    private readonly int _heading;
	
    public ExRotationPacket(int charId, int heading)
    {
        _charId = charId;
        _heading = heading;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ROTATION);
        
        writer.WriteInt32(_charId);
        writer.WriteInt32(_heading);
    }
}