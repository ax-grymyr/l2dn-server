using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExDuelAskStartPacket: IOutgoingPacket
{
    private readonly String _requestorName;
    private readonly int _partyDuel;
	
    public ExDuelAskStartPacket(String requestor, int partyDuel)
    {
        _requestorName = requestor;
        _partyDuel = partyDuel;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_DUEL_ASK_START);
        
        writer.WriteString(_requestorName);
        writer.WriteInt32(_partyDuel);
    }
}