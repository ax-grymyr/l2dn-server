using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExBrPremiumStatePacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public ExBrPremiumStatePacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BR_PREMIUM_STATE);
        
        writer.WriteInt32(_player.getObjectId());
        writer.WriteByte(_player.hasPremiumStatus() || _player.getVipTier() > 0);
    }
}