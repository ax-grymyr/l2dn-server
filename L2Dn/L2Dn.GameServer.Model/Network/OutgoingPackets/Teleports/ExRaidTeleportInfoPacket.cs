using L2Dn.Extensions;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Teleports;

public readonly struct ExRaidTeleportInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public ExRaidTeleportInfoPacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RAID_TELEPORT_INFO);
        // TODO: use TimeSpan
        writer.WriteInt32(DateTime.UtcNow.getEpochSecond() -
            _player.getVariables().getLong("LastFreeRaidTeleportTime", 0) < 86400000);
    }
}