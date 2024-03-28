using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct RidePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly bool _mounted;
    private readonly MountType _rideType;
    private readonly int _rideNpcId;
    private readonly Location _loc;
	
    public RidePacket(Player player)
    {
        _objectId = player.getObjectId();
        _mounted = player.isMounted();
        _rideType = player.getMountType();
        _rideNpcId = player.getMountNpcId() + 1000000;
        _loc = player.getLocation();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.RIDE);
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_mounted);
        writer.WriteInt32((int)_rideType);
        writer.WriteInt32(_rideNpcId);
        writer.WriteInt32(_loc.getX());
        writer.WriteInt32(_loc.getY());
        writer.WriteInt32(_loc.getZ());
    }
}