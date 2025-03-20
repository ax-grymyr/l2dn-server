using L2Dn.GameServer.Model.Shuttles;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Shuttle;

public readonly struct ExShuttleInfoPacket: IOutgoingPacket
{
    private readonly Model.Actor.Instances.Shuttle _shuttle;
    private readonly List<ShuttleStop> _stops;

    public ExShuttleInfoPacket(Model.Actor.Instances.Shuttle shuttle)
    {
        _shuttle = shuttle;
        _stops = shuttle.getStops();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHUTTLE_INFO);
        writer.WriteInt32(_shuttle.ObjectId);
        writer.WriteLocation(_shuttle.Location);
        writer.WriteInt32(_shuttle.Id);
        writer.WriteInt32(_stops.Count);
        foreach (ShuttleStop stop in _stops)
        {
            writer.WriteInt32(stop.getId());
            foreach (Location3D loc in stop.getDimensions())
                writer.WriteLocation3D(loc);

            writer.WriteInt32(stop.isDoorOpen());
            writer.WriteInt32(stop.hasDoorChanged());
        }
    }
}