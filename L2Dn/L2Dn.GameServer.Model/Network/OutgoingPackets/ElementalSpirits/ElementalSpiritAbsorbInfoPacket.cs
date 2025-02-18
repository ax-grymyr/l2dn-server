using System.Collections.Immutable;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;

public readonly struct ElementalSpiritAbsorbInfoPacket(Player player, ElementalType type): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ELEMENTAL_SPIRIT_ABSORB_INFO);

        ElementalSpirit? spirit = player.getElementalSpirit(type);
        if (spirit == null)
        {
            writer.WriteByte(0);
            writer.WriteByte(0);
            return;
        }

        writer.WriteByte(1);
        writer.WriteByte((byte)type);
        writer.WriteByte(spirit.getStage());
        writer.WriteInt64(spirit.getExperience());
        writer.WriteInt64(spirit.getExperienceToNextLevel()); // NextExp
        writer.WriteInt64(spirit.getExperienceToNextLevel()); // MaxExp
        writer.WriteInt32(spirit.getLevel());
        writer.WriteInt32(spirit.getMaxLevel());

        ImmutableArray<ElementalSpiritAbsorbItemHolder> absorbItems = spirit.getAbsorbItems();
        writer.WriteInt32(absorbItems.Length); // AbsorbCount
        foreach (ElementalSpiritAbsorbItemHolder absorbItem in absorbItems)
        {
            writer.WriteInt32(absorbItem.getId());
            writer.WriteInt32((int)(player.getInventory().getItemByItemId(absorbItem.getId())?.getCount() ?? 0));
            writer.WriteInt32(absorbItem.getExperience());
        }
    }
}