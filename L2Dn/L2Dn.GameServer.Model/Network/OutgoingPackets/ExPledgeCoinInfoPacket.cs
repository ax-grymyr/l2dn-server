using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPledgeCoinInfoPacket: IOutgoingPacket
{
    private readonly long _count;
	
    public ExPledgeCoinInfoPacket(Player player)
    {
        _count = player.getHonorCoins();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_COIN_INFO);
        
        writer.WriteInt64(_count);
    }
}