using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ObservationModePacket: IOutgoingPacket
{
    private readonly Location _loc;
	
    public ObservationModePacket(Location loc)
    {
        _loc = loc;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.OBSERVER_START);

        writer.WriteInt32(_loc.getX());
        writer.WriteInt32(_loc.getY());
        writer.WriteInt32(_loc.getZ());
        writer.WriteInt32(0); // TODO: Find me
        writer.WriteInt32(0xc0); // TODO: Find me
    }
}