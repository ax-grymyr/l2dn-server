using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Shuttle;

public readonly struct ExStopMoveInShuttlePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _boatId;
    private readonly Location _pos;
    private readonly int _heading;
	
    public ExStopMoveInShuttlePacket(Player player, int boatId)
    {
        _objectId = player.getObjectId();
        _boatId = boatId;
        _pos = player.getInVehiclePosition();
        _heading = player.getHeading();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_STOP_MOVE_IN_SHUTTLE);
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_boatId);
        writer.WriteInt32(_pos.getX());
        writer.WriteInt32(_pos.getY());
        writer.WriteInt32(_pos.getZ());
        writer.WriteInt32(_heading);
    }
}