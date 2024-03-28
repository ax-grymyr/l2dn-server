using L2Dn.GameServer.InstanceManagers;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Balok;

public readonly struct BalrogWarBossInfoPacket: IOutgoingPacket
{
    private readonly int _bossState1;
    private readonly int _bossState2;
    private readonly int _bossState3;
    private readonly int _bossState4;
    private readonly int _bossState5;
    private readonly int _finalBossId;
    private readonly int _finalState;
	
    public BalrogWarBossInfoPacket(int balokid, int balokstatus, int boss1, int boss2, int boss3, int boss4, int boss5)
    {
        _finalBossId = balokid + 1000000;
        _finalState = balokstatus;
        _bossState1 = boss1;
        _bossState2 = boss2;
        _bossState3 = boss3;
        _bossState4 = boss4;
        _bossState5 = boss5;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BALROGWAR_BOSSINFO);
        
        long globalpoints = BattleWithBalokManager.getInstance().getGlobalPoints();
        int globalstage = BattleWithBalokManager.getInstance().getGlobalStage();
        if ((globalpoints < 320000) && (globalstage <= 2))
        {
            writer.WriteInt32(1);
            writer.WriteInt32(1);
            writer.WriteInt32(1);
            writer.WriteInt32(1);
            writer.WriteInt32(1);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
        }
        else
        {
            int bossId1 = 25956 + 1000000;
            int bossId2 = 25957 + 1000000;
            int bossId3 = 25958 + 1000000;
            int bossId4 = 0;
            int bossId5 = 0;
            if ((globalpoints >= 800000) && (globalstage >= 3))
            {
                bossId4 = 25959 + 1000000;
                bossId5 = 25960 + 1000000;
            }
			
            writer.WriteInt32(bossId1);
            writer.WriteInt32(bossId2);
            writer.WriteInt32(bossId3);
            writer.WriteInt32(bossId4);
            writer.WriteInt32(bossId5);
            writer.WriteInt32(_bossState1);
            writer.WriteInt32(_bossState2);
            writer.WriteInt32(_bossState3);
            writer.WriteInt32(_bossState4);
            writer.WriteInt32(_bossState5);
            writer.WriteInt32(_finalBossId);
            writer.WriteInt32(_finalState);
        }
    }
}