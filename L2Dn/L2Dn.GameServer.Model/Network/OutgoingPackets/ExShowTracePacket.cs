using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

/// <summary>
/// This packet shows the mouse click particle for 30 seconds on every location.
/// </summary>
public readonly struct ExShowTracePacket(List<Location3D> locations): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_TRACE);

        writer.WriteInt16(0); // type broken in H5
        writer.WriteInt32(0); // time broken in H5
        writer.WriteInt16((short)locations.Count);
        foreach (Location3D loc in locations)
            writer.WriteLocation3D(loc);
    }
}