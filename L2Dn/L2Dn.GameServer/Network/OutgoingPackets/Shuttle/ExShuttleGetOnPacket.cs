using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Shuttle;

public readonly struct ExShuttleGetOnPacket: IOutgoingPacket
{
    private readonly int _playerObjectId;
    private readonly int _shuttleObjectId;
    private readonly Location _pos;
	
    public ExShuttleGetOnPacket(Player player, Model.Actor.Instances.Shuttle shuttle)
    {
        _playerObjectId = player.getObjectId();
        _shuttleObjectId = shuttle.getObjectId();
        _pos = player.getInVehiclePosition();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SUTTLE_GET_ON);
        writer.WriteInt32(_playerObjectId);
        writer.WriteInt32(_shuttleObjectId);
        writer.WriteInt32(_pos.getX());
        writer.WriteInt32(_pos.getY());
        writer.WriteInt32(_pos.getZ());
    }
}