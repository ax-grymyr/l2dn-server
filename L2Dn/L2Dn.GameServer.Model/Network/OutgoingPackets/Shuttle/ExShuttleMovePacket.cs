using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Shuttle;

public readonly struct ExShuttleMovePacket: IOutgoingPacket
{
    private readonly Model.Actor.Instances.Shuttle _shuttle;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
	
    public ExShuttleMovePacket(Model.Actor.Instances.Shuttle shuttle, int x, int y, int z)
    {
        _shuttle = shuttle;
        _x = x;
        _y = y;
        _z = z;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SUTTLE_MOVE);

        writer.WriteInt32(_shuttle.getObjectId());
        writer.WriteInt32((int)_shuttle.getStat().getMoveSpeed());
        writer.WriteInt32((int)_shuttle.getStat().getRotationSpeed());
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
    }
}