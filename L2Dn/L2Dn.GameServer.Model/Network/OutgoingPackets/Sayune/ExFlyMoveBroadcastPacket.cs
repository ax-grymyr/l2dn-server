using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Sayune;

public readonly struct ExFlyMoveBroadcastPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _mapId;
    private readonly Location3D _currentLoc;
    private readonly Location3D _targetLoc;
    private readonly SayuneType _type;
	
    public ExFlyMoveBroadcastPacket(Player player, SayuneType type, int mapId, Location3D targetLoc)
    {
        _objectId = player.getObjectId();
        _type = type;
        _mapId = mapId;
        _currentLoc = player.getLocation().ToLocation3D();
        _targetLoc = targetLoc;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_FLY_MOVE_BROADCAST);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32((int)_type);
        writer.WriteInt32(_mapId);
        writer.WriteLocation3D(_targetLoc);
        writer.WriteInt32(0); // ?
        writer.WriteLocation3D(_currentLoc);
    }
}