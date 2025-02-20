using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.OutgoingPackets.RaidBossInfo;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.RaidBosses;

public struct RequestRaidBossSpawnInfoPacket: IIncomingPacket<GameSession>
{
    private const int BAIUM = 29020;

    private int[]? _bossIds;

    public void ReadContent(PacketBitReader reader)
    {
        int count = reader.ReadInt32();
        if (count > 0 && count < 20000)
        {
            _bossIds = new int[count];
            for (int i = 0; i < count; i++)
                _bossIds[i] = reader.ReadInt32();
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (_bossIds == null)
            return ValueTask.CompletedTask;

        Map<int, RaidBossStatus> statuses = new();

        foreach (var bossId in _bossIds)
        {
            GrandBoss? boss = GrandBossManager.getInstance().getBoss(bossId);
            if (boss == null)
            {
                RaidBossStatus status = DbSpawnManager.getInstance().getStatus(bossId);
                if (status != RaidBossStatus.UNDEFINED)
                {
                    Npc? npc = DbSpawnManager.getInstance().getNpc(bossId);
                    if (npc != null && npc.isInCombat())
                    {
                        statuses.put(bossId, RaidBossStatus.COMBAT);
                    }
                    else
                    {
                        statuses.put(bossId, status);
                    }
                }
                else
                {
                    statuses.put(bossId, RaidBossStatus.DEAD);
                    // PacketLogger.warning("Could not find spawn info for boss " + bossId + ".");
                }
            }
            else
            {
                if (boss.isDead() || !boss.isSpawned())
                {
                    if (bossId == BAIUM && GrandBossManager.getInstance().getStatus(BAIUM) == 0)
                    {
                        statuses.put(bossId, RaidBossStatus.ALIVE);
                    }
                    else
                    {
                        statuses.put(bossId, RaidBossStatus.DEAD);
                    }
                }
                else if (boss.isInCombat())
                {
                    statuses.put(bossId, RaidBossStatus.COMBAT);
                }
                else
                {
                    statuses.put(bossId, RaidBossStatus.ALIVE);
                }
            }
        }

        connection.Send(new ExRaidBossSpawnInfoPacket(statuses));

        return ValueTask.CompletedTask;
    }
}