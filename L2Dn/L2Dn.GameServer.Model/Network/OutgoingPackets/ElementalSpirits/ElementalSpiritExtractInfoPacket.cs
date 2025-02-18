using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;

public readonly struct ElementalSpiritExtractInfoPacket(Player player, ElementalType type): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ELEMENTAL_SPIRIT_EXTRACT_INFO);

        ElementalSpirit? spirit = player.getElementalSpirit(type);
        if (spirit == null)
        {
            writer.WriteByte(0);
            writer.WriteByte(0);
            return;
        }

        writer.WriteByte((byte)type); // active elemental spirit
        writer.WriteByte(1); // is extract ?
        writer.WriteByte(1); // cost count

        // for each cost count
        writer.WriteInt32(57); // item id
        writer.WriteInt32(ElementalSpiritData.ExtractFees[spirit.getStage() - 1]); // item count
        writer.WriteInt32(spirit.getExtractItem());
        writer.WriteInt32(spirit.getExtractAmount());
    }
}