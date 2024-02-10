using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct CharacterSelectedPacket(int playKey1, Character character): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x0B); // packet code (0x15 in C4)

        CharacterClassInfo classInfo = StaticData.Templates[character.Class];
        (int level, decimal percents) = StaticData.Levels.GetLevelForExp(character.Exp);
        
        writer.WriteString(character.Name); // character name
        writer.WriteInt32(character.Id); // character id
        writer.WriteString(character.Title); // character title
        writer.WriteInt32(playKey1);
        writer.WriteInt32(0); // clan id
        writer.WriteInt32(0); // ??
        writer.WriteInt32((int)character.Sex); // 0 - male, 1 - female
        writer.WriteInt32((int)classInfo.Race); // race
        writer.WriteInt32((int)character.Class); // class
        writer.WriteInt32(1); // active ??
        writer.WriteInt32(character.LocationX); // x
        writer.WriteInt32(character.LocationY); // y
        writer.WriteInt32(character.LocationZ); // z
        writer.WriteDouble(character.CurrentHp); // current HP
        writer.WriteDouble(character.CurrentMp); // current MP
        writer.WriteInt64(character.Sp); // SP
        writer.WriteInt64(character.Exp); // EXP
        writer.WriteInt32(level); // level
        writer.WriteInt32(character.Reputation); // karma
        writer.WriteInt32(character.PkCounter); // pk kills
        writer.WriteInt32(0); // "reset" on 24th hour: GameTimeTaskManager.getInstance().getGameTime() % (24 * 60)
        writer.WriteInt32(0);
        writer.WriteInt32((int)character.Class);
        writer.WriteZeros(16);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteZeros(28);
        writer.WriteInt32(0);

        // C4
        // writer.WriteInt32(baseStats.Int); // INT
        // writer.WriteInt32(baseStats.Str); // STR
        // writer.WriteInt32(baseStats.Con); // CON
        // writer.WriteInt32(baseStats.Men); // MEN
        // writer.WriteInt32(baseStats.Dex); // DEX
        // writer.WriteInt32(baseStats.Wit); // WIT
        //
        // for (int i = 0; i < 30; i++)
        //     writer.WriteInt32(0);
        //
        // // writer.WriteInt32(0); //c3
        // // writer.WriteInt32(0); //c3
        // // writer.WriteInt32(0); //c3
        // writer.WriteInt32(0); // c3 work
        // writer.WriteInt32(0); // c3 work
        //
        // // extra info
        // writer.WriteInt32(0); // in-game time
        // writer.WriteInt32(0); //
        // writer.WriteInt32(0); // c3
        // writer.WriteInt32(0); // c3 InspectorBin
        // writer.WriteInt32(0); // c3
        // writer.WriteInt32(0); // c3
        // writer.WriteInt32(0); // c3
        // writer.WriteInt32(0); // c3 InspectorBin for 528 client
        // writer.WriteInt32(0); // c3
        // writer.WriteInt32(0); // c3
        // writer.WriteInt32(0); // c3
        // writer.WriteInt32(0); // c3
        // writer.WriteInt32(0); // c3
        // writer.WriteInt32(0); // c3
        // writer.WriteInt32(0); // c3
    }
}
