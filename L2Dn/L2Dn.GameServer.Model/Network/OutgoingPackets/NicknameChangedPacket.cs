using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct NicknameChangedPacket: IOutgoingPacket
{
    private readonly string _title;
    private readonly int _objectId;
	
    public NicknameChangedPacket(Creature creature)
    {
        _objectId = creature.getObjectId();
        _title = creature.getTitle();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.NICK_NAME_CHANGED);

        writer.WriteInt32(_objectId);
        writer.WriteString(_title);
    }
}