using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct AskJoinPledgePacket: IOutgoingPacket
{
    private readonly Player _requestor;
    private readonly string _pledgeName;
	
    public AskJoinPledgePacket(Player requestor, string pledgeName)
    {
        _requestor = requestor;
        _pledgeName = pledgeName;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.ASK_JOIN_PLEDGE);
        
        writer.WriteInt32(_requestor.getObjectId());
        writer.WriteString("");
        writer.WriteString(_pledgeName);
        writer.WriteInt32(0);
        writer.WriteString("");
    }
}