using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct NewCharacterSuccessPacket: IOutgoingPacket
{
    private readonly List<PlayerTemplate> _templates;

    public NewCharacterSuccessPacket(List<PlayerTemplate> templates)
    {
        _templates = templates;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.NEW_CHARACTER_SUCCESS);

        writer.WriteInt32(_templates.Count);
        foreach (PlayerTemplate chr in _templates)
        {
            // TODO: Unhardcode these
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