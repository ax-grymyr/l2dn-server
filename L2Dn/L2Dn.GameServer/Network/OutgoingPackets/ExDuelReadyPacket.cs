using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExDuelReadyPacket: IOutgoingPacket
{
    public static readonly ExDuelReadyPacket PLAYER_DUEL = new(false);
    public static readonly ExDuelReadyPacket PARTY_DUEL = new(true);
	
    private readonly bool _partyDuel;
	
    public ExDuelReadyPacket(bool isPartyDuel)
    {
        _partyDuel = isPartyDuel;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_DUEL_READY);
        writer.WriteInt32(_partyDuel);
    }
}