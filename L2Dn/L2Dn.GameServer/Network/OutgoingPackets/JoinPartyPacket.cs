using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct JoinPartyPacket: IOutgoingPacket
{
    private readonly int _response;
    private readonly int _type;
	
    public JoinPartyPacket(int response, Player requestor)
    {
        _response = response;
        _type = requestor.getClientSettings().getPartyContributionType();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.JOIN_PARTY);
        
        writer.WriteInt32(_response);
        writer.WriteInt32(_type);
        if (_type != 0)
        {
            writer.WriteInt32(0); // TODO: Find me!
            writer.WriteInt32(0); // TODO: Find me!
        }
    }
}