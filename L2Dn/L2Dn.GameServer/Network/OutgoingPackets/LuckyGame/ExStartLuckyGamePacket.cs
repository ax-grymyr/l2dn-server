using L2Dn.GameServer.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.LuckyGame;

public readonly struct ExStartLuckyGamePacket: IOutgoingPacket
{
    private readonly LuckyGameType _type;
    private readonly int _ticketCount;
	
    public ExStartLuckyGamePacket(LuckyGameType type, long ticketCount)
    {
        _type = type;
        _ticketCount = (int)ticketCount;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_START_LUCKY_GAME);
        
        writer.WriteInt32((int)_type);
        writer.WriteInt32(_ticketCount);
    }
}