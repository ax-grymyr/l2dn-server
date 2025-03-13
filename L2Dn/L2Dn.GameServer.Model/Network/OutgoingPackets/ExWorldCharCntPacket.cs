using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.StaticData;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExWorldCharCntPacket: IOutgoingPacket
{
    private readonly int _points;

    public ExWorldCharCntPacket(Player player)
    {
        _points = player.getLevel() < Config.WORLD_CHAT_MIN_LEVEL ||
                  (Config.VIP_SYSTEM_ENABLED && player.getVipTier() <= 0)
            ? 0
            : Math.Max(player.getWorldChatPoints() - player.getWorldChatUsed(), 0);
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_WORLD_CHAT_CNT);
        writer.WriteInt32(_points);
    }
}