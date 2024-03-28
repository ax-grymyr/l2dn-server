using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ObservationReturnPacket: IOutgoingPacket
{
    private readonly Location _loc;
	
    public ObservationReturnPacket(Location loc)
    {
        _loc = loc;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.OBSERVER_END);

        writer.WriteInt32(_loc.getX());
        writer.WriteInt32(_loc.getY());
        writer.WriteInt32(_loc.getZ());
    }
}