using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct NewCharacterSuccessPacket(CharacterClass[] classes): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x0D); // packet code

        writer.WriteInt32(classes.Length);
        foreach (CharacterClass characterClass in classes)
        {
            CharacterClassInfo classInfo = StaticData.Templates[characterClass];
            CharacterSpecData spec = StaticData.Templates[classInfo.Race][classInfo.Spec];
            CharacterBaseStats baseStats = spec.BaseStats;

            writer.WriteInt32((int)classInfo.Race);
            writer.WriteInt32((int)characterClass);
            writer.WriteInt32(99);
            writer.WriteInt32(baseStats.Str);
            writer.WriteInt32(1);
            writer.WriteInt32(99);
            writer.WriteInt32(baseStats.Dex);
            writer.WriteInt32(1);
            writer.WriteInt32(99);
            writer.WriteInt32(baseStats.Con);
            writer.WriteInt32(1);
            writer.WriteInt32(99);
            writer.WriteInt32(baseStats.Int);
            writer.WriteInt32(1);
            writer.WriteInt32(99);
            writer.WriteInt32(baseStats.Wit);
            writer.WriteInt32(1);
            writer.WriteInt32(99);
            writer.WriteInt32(baseStats.Men);
            writer.WriteInt32(1);
        }
    }
}
