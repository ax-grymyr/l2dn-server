using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExDuelEndPacket: IOutgoingPacket
{
    public static readonly ExDuelEndPacket PLAYER_DUEL = new(false);
    public static readonly ExDuelEndPacket PARTY_DUEL = new(true);
	
    private readonly bool _partyDuel;
	
    public ExDuelEndPacket(bool isPartyDuel)
    {
        _partyDuel = isPartyDuel;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_DUEL_END);
        writer.WriteInt32(_partyDuel);
    }
}