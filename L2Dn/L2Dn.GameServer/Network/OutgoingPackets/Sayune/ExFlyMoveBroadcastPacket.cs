using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Sayune;

public readonly struct ExFlyMoveBroadcastPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _mapId;
    private readonly ILocational _currentLoc;
    private readonly ILocational _targetLoc;
    private readonly SayuneType _type;
	
    public ExFlyMoveBroadcastPacket(Player player, SayuneType type, int mapId, ILocational targetLoc)
    {
        _objectId = player.getObjectId();
        _type = type;
        _mapId = mapId;
        _currentLoc = player;
        _targetLoc = targetLoc;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_FLY_MOVE_BROADCAST);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32((int)_type);
        writer.WriteInt32(_mapId);
        writer.WriteInt32(_targetLoc.getX());
        writer.WriteInt32(_targetLoc.getY());
        writer.WriteInt32(_targetLoc.getZ());
        writer.WriteInt32(0); // ?
        writer.WriteInt32(_currentLoc.getX());
        writer.WriteInt32(_currentLoc.getY());
        writer.WriteInt32(_currentLoc.getZ());
    }
}