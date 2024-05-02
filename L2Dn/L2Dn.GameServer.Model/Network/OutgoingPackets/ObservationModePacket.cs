using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ObservationModePacket: IOutgoingPacket
{
    private readonly Location3D _location;

    public ObservationModePacket(Location3D location)
    {
        _location = location;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.OBSERVER_START);

        writer.WriteLocation3D(_location);
        writer.WriteInt32(0); // TODO: Find me
        writer.WriteInt32(0xc0); // TODO: Find me
    }
}