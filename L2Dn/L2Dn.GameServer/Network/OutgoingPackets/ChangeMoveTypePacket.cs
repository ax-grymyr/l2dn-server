using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ChangeMoveTypePacket: IOutgoingPacket
{
    public const int WALK = 0;
    public const int RUN = 1;
	
    private readonly int _objectId;
    private readonly bool _running;
	
    public ChangeMoveTypePacket(Creature creature)
    {
        _objectId = creature.getObjectId();
        _running = creature.isRunning();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.CHANGE_MOVE_TYPE);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_running ? RUN : WALK);
        writer.WriteInt32(0); // c2
    }
}