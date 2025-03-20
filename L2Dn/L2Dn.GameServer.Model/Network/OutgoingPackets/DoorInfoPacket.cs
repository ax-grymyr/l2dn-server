using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct DoorInfoPacket: IOutgoingPacket
{
    private readonly Door _door;

    public DoorInfoPacket(Door door)
    {
        _door = door;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.DOOR_INFO);

        writer.WriteInt32(_door.ObjectId);
        writer.WriteInt32(_door.Id);
    }
}