using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

/// <summary>
/// This packet shows the mouse click particle for 30 seconds on every location.
/// </summary>
public readonly struct ExShowTracePacket: IOutgoingPacket
{
    private readonly List<Location> _locations;

    public ExShowTracePacket(List<Location> locations)
    {
        _locations = locations;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_TRACE);
        
        writer.WriteInt16(0); // type broken in H5
        writer.WriteInt32(0); // time broken in H5
        writer.WriteInt16((short)_locations.Count);
        foreach (Location loc in _locations)
        {
            writer.WriteInt32(loc.getX());
            writer.WriteInt32(loc.getY());
            writer.WriteInt32(loc.getZ());
        }
    }
}