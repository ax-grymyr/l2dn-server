using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Shuttle;

public readonly struct ExShuttleMovePacket: IOutgoingPacket
{
    private readonly Model.Actor.Instances.Shuttle _shuttle;
    private readonly Location3D _location;

    public ExShuttleMovePacket(Model.Actor.Instances.Shuttle shuttle, Location3D location)
    {
        _shuttle = shuttle;
        _location = location;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SUTTLE_MOVE);

        writer.WriteInt32(_shuttle.ObjectId);
        writer.WriteInt32((int)_shuttle.getStat().getMoveSpeed());
        writer.WriteInt32((int)_shuttle.getStat().getRotationSpeed());
        writer.WriteLocation3D(_location);
    }
}