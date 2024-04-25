using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Sayune;

public readonly struct ExFlyMovePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly SayuneType _type;
    private readonly int _mapId;
    private readonly List<SayuneEntry> _locations;
	
    public ExFlyMovePacket(Player player, SayuneType type, int mapId, List<SayuneEntry> locations)
    {
        _objectId = player.getObjectId();
        _type = type;
        _mapId = mapId;
        _locations = locations;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_FLY_MOVE);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32((int)_type);
        writer.WriteInt32(0); // ??
        writer.WriteInt32(_mapId);
        writer.WriteInt32(_locations.Count);
        foreach (SayuneEntry sayuneEntry in _locations)
        {
            writer.WriteInt32(sayuneEntry.getId());
            writer.WriteInt32(0); // ??
            writer.WriteLocation3D(sayuneEntry.Location);
        }
    }
}