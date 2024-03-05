using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.RaidBossInfo;

public readonly struct ExRaidBossSpawnInfoPacket: IOutgoingPacket
{
    private readonly Map<int, RaidBossStatus> _statuses;
	
    public ExRaidBossSpawnInfoPacket(Map<int, RaidBossStatus> statuses)
    {
        _statuses = statuses;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RAID_BOSS_SPAWN_INFO);
        
        writer.WriteInt32(0); // BossRespawnFactor
        writer.WriteInt32(_statuses.size()); // count
        foreach (var entry in _statuses)
        {
            writer.WriteInt32(entry.Key);
            writer.WriteInt32((int)entry.Value);
            writer.WriteInt32(0); // DeadDateTime
        }
    }
}