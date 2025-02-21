using System.Collections.Immutable;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct NewCharacterSuccessPacket(ImmutableArray<PlayerTemplate> templates): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.NEW_CHARACTER_SUCCESS);

        if (templates.IsDefaultOrEmpty)
        {
            writer.WriteInt32(0);
            return;
        }

        writer.WriteInt32(templates.Length);
        foreach (PlayerTemplate chr in templates)
        {
            // TODO: Unhardcode numbers
            writer.WriteInt32((int)chr.getRace());
            writer.WriteInt32((int)chr.getClassId());
            writer.WriteInt32(99);
            writer.WriteInt32(chr.getBaseSTR());
            writer.WriteInt32(1);
            writer.WriteInt32(99);
            writer.WriteInt32(chr.getBaseDEX());
            writer.WriteInt32(1);
            writer.WriteInt32(99);
            writer.WriteInt32(chr.getBaseCON());
            writer.WriteInt32(1);
            writer.WriteInt32(99);
            writer.WriteInt32(chr.getBaseINT());
            writer.WriteInt32(1);
            writer.WriteInt32(99);
            writer.WriteInt32(chr.getBaseWIT());
            writer.WriteInt32(1);
            writer.WriteInt32(99);
            writer.WriteInt32(chr.getBaseMEN());
            writer.WriteInt32(1);
        }
    }
}