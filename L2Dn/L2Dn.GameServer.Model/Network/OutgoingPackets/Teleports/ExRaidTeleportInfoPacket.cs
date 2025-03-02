using L2Dn.Extensions;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Teleports;

public readonly struct ExRaidTeleportInfoPacket(Player player): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RAID_TELEPORT_INFO);
        // TODO: use TimeSpan
        writer.WriteInt32(DateTime.UtcNow.getEpochSecond() -
            player.getVariables().Get("LastFreeRaidTeleportTime", 0L) < 86400000);
    }
}