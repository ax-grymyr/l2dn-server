using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Enchant.Attributes;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.WorldExchange;

public readonly struct WorldExchangeItemListPacket: IOutgoingPacket
{
    public static readonly WorldExchangeItemListPacket EMPTY_LIST = new(new List<WorldExchangeHolder>(), default);

    private readonly List<WorldExchangeHolder>? _holders;
    private readonly WorldExchangeItemSubType _type;

    public WorldExchangeItemListPacket(List<WorldExchangeHolder> holders, WorldExchangeItemSubType type)
    {
        _holders = holders;
        _type = type;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_WORLD_EXCHANGE_ITEM_LIST);

        if (_holders is null || _holders.Count == 0)
        {
            writer.WriteInt16(0); // Category
            writer.WriteByte(0); // SortType
            writer.WriteInt32(0); // Page
            writer.WriteInt32(0); // ItemIDList
            return;
        }

        writer.WriteInt16((short)_type);
        writer.WriteByte(0);
        writer.WriteInt32(0);
        writer.WriteInt32(_holders.Count);
        foreach (WorldExchangeHolder holder in _holders)
            getItemInfo(writer, holder);
    }

    private void getItemInfo(PacketBitWriter writer, WorldExchangeHolder holder)
    {
        writer.WriteInt64(holder.getWorldExchangeId());
        writer.WriteInt64(holder.getPrice());
        writer.WriteInt32(holder.getEndTime().getEpochSecond());

        Item item = holder.getItemInstance();
        writer.WriteInt32(item.Id);
        writer.WriteInt64(item.getCount());
        writer.WriteInt32(item.getEnchantLevel() < 1 ? 0 : item.getEnchantLevel());
        VariationInstance? iv = item.getAugmentation();
        writer.WriteInt32(iv?.getOption1Id() ?? 0);
        writer.WriteInt32(iv?.getOption2Id() ?? 0);
        writer.WriteInt32(-1);
        AttributeHolder? attackAttribute = item.getAttackAttribute();
        writer.WriteInt16(attackAttribute != null ? (short)attackAttribute.getType() : (short)0);
        writer.WriteInt16(attackAttribute != null ? (short)attackAttribute.getValue() : (short)0);
        writer.WriteInt16((short)item.getDefenceAttribute(AttributeType.FIRE));
        writer.WriteInt16((short)item.getDefenceAttribute(AttributeType.WATER));
        writer.WriteInt16((short)item.getDefenceAttribute(AttributeType.WIND));
        writer.WriteInt16((short)item.getDefenceAttribute(AttributeType.EARTH));
        writer.WriteInt16((short)item.getDefenceAttribute(AttributeType.HOLY));
        writer.WriteInt16((short)item.getDefenceAttribute(AttributeType.DARK));
        writer.WriteInt32(item.getVisualId());

        List<EnsoulOption> soul = (List<EnsoulOption>)holder.getItemInfo().getSoulCrystalOptions();
        try
        {
            writer.WriteInt32(soul != null ? soul[0].getId() : 0);
        }
        catch (IndexOutOfRangeException)
        {
            writer.WriteInt32(0);
        }

        try
        {
            writer.WriteInt32(soul != null ? soul[1].getId() : 0);
        }
        catch (IndexOutOfRangeException)
        {
            writer.WriteInt32(0);
        }

        List<EnsoulOption> specialSoul = (List<EnsoulOption>)holder.getItemInfo().getSoulCrystalSpecialOptions();
        try
        {
            writer.WriteInt32(specialSoul != null ? specialSoul[0].getId() : 0);
        }
        catch (IndexOutOfRangeException)
        {
            writer.WriteInt32(0);
        }

        writer.WriteInt16(item.isBlessed());
    }
}