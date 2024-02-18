using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Shuttle;

public readonly struct ExShuttleGetOffPacket: IOutgoingPacket
{
    private readonly int _playerObjectId;
    private readonly int _shuttleObjectId;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
	
    public ExShuttleGetOffPacket(Player player, Model.Actor.Instances.Shuttle shuttle, int x, int y, int z)
    {
        _playerObjectId = player.getObjectId();
        _shuttleObjectId = shuttle.getObjectId();
        _x = x;
        _y = y;
        _z = z;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SUTTLE_GET_OFF);
        writer.WriteInt32(_playerObjectId);
        writer.WriteInt32(_shuttleObjectId);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
    }
}