using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Subjugation;

public readonly struct ExSubjugationSidebarPacket: IOutgoingPacket
{
    private readonly Player? _player;
    private readonly PurgePlayerHolder? _purgeData;
	
    public ExSubjugationSidebarPacket(Player? player, PurgePlayerHolder? purgeData)
    {
        _player = player;
        _purgeData = purgeData;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SUBJUGATION_SIDEBAR);
        writer.WriteInt32(_player?.getPurgeLastCategory() ?? 0);
        writer.WriteInt32(_purgeData?.getPoints() ?? 0); // 1000000 = 100 percent
        writer.WriteInt32(_purgeData?.getKeys() ?? 0);
        writer.WriteInt32(0);
    }
}