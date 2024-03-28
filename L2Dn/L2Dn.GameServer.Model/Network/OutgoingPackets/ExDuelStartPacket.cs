using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExDuelStartPacket: IOutgoingPacket
{
    public static readonly ExDuelStartPacket PLAYER_DUEL = new(false);
    public static readonly ExDuelStartPacket PARTY_DUEL = new(true);
	
    private readonly bool _partyDuel;
	
    public ExDuelStartPacket(bool isPartyDuel)
    {
        _partyDuel = isPartyDuel;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_DUEL_START);
        writer.WriteInt32(_partyDuel);
    }
}