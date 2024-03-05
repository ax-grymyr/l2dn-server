using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.RaidBossInfo;

public readonly struct ExRaidServerInfoPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RAID_SERVER_INFO);
    }
}