using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Vips;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Vip;

public readonly struct ReceiveVipInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public ReceiveVipInfoPacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        if (!Config.VIP_SYSTEM_ENABLED)
        {
            return;
        }
		
        VipManager vipManager = VipManager.getInstance();
        byte vipTier = (byte)_player.getVipTier();

        DateTime? expiration = _player.getVipTierExpiration();
        int vipDuration = expiration is null ? 0 : (int)(expiration.Value - DateTime.UtcNow).TotalSeconds;
		
        writer.WritePacketCode(OutgoingPacketCodes.RECIVE_VIP_INFO);
        
        writer.WriteByte(vipTier);
        writer.WriteInt64(_player.getVipPoints());
        writer.WriteInt32(vipDuration);
        writer.WriteInt64(vipManager.getPointsToLevel((byte)(vipTier + 1)));
        writer.WriteInt64(vipManager.getPointsDepreciatedOnLevel(vipTier));
        writer.WriteByte(vipTier);
        writer.WriteInt64(vipManager.getPointsToLevel(vipTier));
    }
}