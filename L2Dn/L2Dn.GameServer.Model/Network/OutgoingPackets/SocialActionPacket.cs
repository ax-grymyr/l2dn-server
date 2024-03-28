using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SocialActionPacket: IOutgoingPacket
{
    // TODO: Enum
    public const int LEVEL_UP = 2122;
	
    private readonly int _objectId;
    private readonly int _actionId;
	
    public SocialActionPacket(int objectId, int actionId)
    {
        _objectId = objectId;
        _actionId = actionId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SOCIAL_ACTION);
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_actionId);
        writer.WriteInt32(0); // TODO: Find me!
    }
}