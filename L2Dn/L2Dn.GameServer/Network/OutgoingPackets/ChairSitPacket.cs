using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ChairSitPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _staticObjectId;
	
    public ChairSitPacket(Player player, int staticObjectId)
    {
        _player = player;
        _staticObjectId = staticObjectId;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.CHAIR_SIT);
        
        writer.WriteInt32(_player.getObjectId());
        writer.WriteInt32(_staticObjectId);
    }
}