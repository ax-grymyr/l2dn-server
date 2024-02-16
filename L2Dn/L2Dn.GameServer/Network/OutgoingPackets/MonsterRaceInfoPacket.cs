using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public struct MonsterRaceInfoPacket: IOutgoingPacket
{
    private readonly int _unknown1;
    private readonly int _unknown2;
    private readonly Npc[] _monsters;
    private readonly int[][] _speeds;

    public MonsterRaceInfoPacket(int unknown1, int unknown2, Npc[] monsters, int[][] speeds)
    {
        /*
         * -1 0 to initial the race 0 15322 to start race 13765 -1 in middle of race -1 0 to end the race
         */
        _unknown1 = unknown1;
        _unknown2 = unknown2;
        _monsters = monsters;
        _speeds = speeds;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.MONSTER_RACE_INFO);
        writer.WriteInt32(_unknown1);
        writer.WriteInt32(_unknown2);
        writer.WriteInt32(8);
        for (int i = 0; i < 8; i++)
        {
            writer.WriteInt32(_monsters[i].getObjectId()); // npcObjectID
            writer.WriteInt32(_monsters[i].getTemplate().getDisplayId() + 1000000); // npcID
            writer.WriteInt32(14107); // origin X
            writer.WriteInt32(181875 + (58 * (7 - i))); // origin Y
            writer.WriteInt32(-3566); // origin Z
            writer.WriteInt32(12080); // end X
            writer.WriteInt32(181875 + (58 * (7 - i))); // end Y
            writer.WriteInt32(-3566); // end Z
            writer.WriteDouble(_monsters[i].getTemplate().getFCollisionHeight()); // coll. height
            writer.WriteDouble(_monsters[i].getTemplate().getFCollisionRadius()); // coll. radius
            writer.WriteInt32(120); // ?? unknown
            
            for (int j = 0; j < 20; j++)
            {
                if (_unknown1 == 0)
                {
                    writer.WriteByte((byte)_speeds[i][j]);
                }
                else
                {
                    writer.WriteByte(0);
                }
            }
        }
    }
}