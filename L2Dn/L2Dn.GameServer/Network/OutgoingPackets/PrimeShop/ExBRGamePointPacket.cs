using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.PrimeShop;

public readonly struct ExBRGamePointPacket: IOutgoingPacket
{
    private readonly int _charId;
    private readonly int _charPoints;
	
    public ExBRGamePointPacket(Player player)
    {
        _charId = player.getObjectId();
        _charPoints = player.getPrimePoints();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BR_GAME_POINT);
        
        writer.WriteInt32(_charId);
        writer.WriteInt64(_charPoints);
        writer.WriteInt32(0);
    }
}