using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct RevivePacket: IOutgoingPacket
{
    private readonly int _objectId;
	
    public RevivePacket(WorldObject obj)
    {
        _objectId = obj.ObjectId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.REVIVE);
        
        writer.WriteInt32(_objectId);
    }
}