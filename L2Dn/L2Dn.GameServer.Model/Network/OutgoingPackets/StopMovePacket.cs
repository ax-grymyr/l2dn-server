using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct StopMovePacket(int objectId, int x, int y, int z, int heading)
    : IOutgoingPacket
{
    public StopMovePacket(Creature creature): this(creature.ObjectId, creature.getX(), creature.getY(), creature.getZ(), creature.getHeading())
    {
    }
    
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.STOP_MOVE);
        
        writer.WriteInt32(objectId);
        writer.WriteInt32(x);
        writer.WriteInt32(y);
        writer.WriteInt32(z);
        writer.WriteInt32(heading);
    }
}