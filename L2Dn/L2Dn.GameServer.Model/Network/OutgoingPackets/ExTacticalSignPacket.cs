using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExTacticalSignPacket: IOutgoingPacket
{
    private readonly Creature _target;
    private readonly int _tokenId;
	
    public ExTacticalSignPacket(Creature target, int tokenId)
    {
        _target = target;
        _tokenId = tokenId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_TACTICAL_SIGN);
        
        writer.WriteInt32(_target.getObjectId());
        writer.WriteInt32(_tokenId);
    }
}