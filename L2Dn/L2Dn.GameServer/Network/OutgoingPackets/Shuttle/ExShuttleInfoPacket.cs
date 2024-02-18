using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Shuttles;
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
        writer.WriteInt32(_shuttle.getObjectId());
        writer.WriteInt32(_shuttle.getX());
        writer.WriteInt32(_shuttle.getY());
        writer.WriteInt32(_shuttle.getZ());
        writer.WriteInt32(_shuttle.getHeading());
        writer.WriteInt32(_shuttle.getId());
        writer.WriteInt32(_stops.Count);
        foreach (ShuttleStop stop in _stops)
        {
            writer.WriteInt32(stop.getId());
            foreach (Location loc in stop.getDimensions())
            {
                writer.WriteInt32(loc.getX());
                writer.WriteInt32(loc.getY());
                writer.WriteInt32(loc.getZ());
            }
            writer.WriteInt32(stop.isDoorOpen());
            writer.WriteInt32(stop.hasDoorChanged());
        }
    }
}