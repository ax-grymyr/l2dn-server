using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowFortressMapInfoPacket: IOutgoingPacket
{
    private readonly Fort _fortress;
	
    public ExShowFortressMapInfoPacket(Fort fortress)
    {
        _fortress = fortress;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_FORTRESS_MAP_INFO);
        
        writer.WriteInt32(_fortress.getResidenceId());
        writer.WriteInt32(_fortress.getSiege().isInProgress()); // fortress siege status
        writer.WriteInt32(_fortress.getFortSize()); // barracks count
        List<FortSiegeSpawn> commanders = FortSiegeManager.getInstance().getCommanderSpawnList(_fortress.getResidenceId());
        if (commanders != null && commanders.Count != 0 && _fortress.getSiege().isInProgress())
        {
            switch (commanders.Count)
            {
                case 3:
                {
                    foreach (FortSiegeSpawn spawn in commanders)
                    {
                        if (isSpawned(spawn.getId()))
                        {
                            writer.WriteInt32(0);
                        }
                        else
                        {
                            writer.WriteInt32(1);
                        }
                    }
                    break;
                }
                case 4: // TODO: change 4 to 5 once control room supported
                {
                    int count = 0;
                    foreach (FortSiegeSpawn spawn in commanders)
                    {
                        count++;
                        if (count == 4)
                        {
                            writer.WriteInt32(1); // TODO: control room emulated
                        }
                        if (isSpawned(spawn.getId()))
                        {
                            writer.WriteInt32(0);
                        }
                        else
                        {
                            writer.WriteInt32(1);
                        }
                    }
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < _fortress.getFortSize(); i++)
            {
                writer.WriteInt32(0);
            }
        }
    }
	
    /**
     * @param npcId
     * @return
     */
    private bool isSpawned(int npcId)
    {
        bool ret = false;
        foreach (Spawn spawn in _fortress.getSiege().getCommanders())
        {
            if (spawn.getId() == npcId)
            {
                ret = true;
                break;
            }
        }
        return ret;
    }
}