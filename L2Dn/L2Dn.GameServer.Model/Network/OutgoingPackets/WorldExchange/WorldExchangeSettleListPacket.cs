using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.WorldExchange;

public readonly struct WorldExchangeSettleListPacket(Player player): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_WORLD_EXCHANGE_SETTLE_LIST);

        Map<WorldExchangeItemStatusType, List<WorldExchangeHolder>> holders =
            WorldExchangeManager.getInstance().getPlayerBids(player.ObjectId);

        if (holders.Count == 0)
        {
            writer.WriteInt32(0); // RegiItemDataList
            writer.WriteInt32(0); // RecvItemDataList
            writer.WriteInt32(0); // TimeOutItemDataList
            return;
        }

        WriteItems(writer, holders, WorldExchangeItemStatusType.WORLD_EXCHANGE_REGISTERED);
        WriteItems(writer, holders, WorldExchangeItemStatusType.WORLD_EXCHANGE_SOLD);
        WriteItems(writer, holders, WorldExchangeItemStatusType.WORLD_EXCHANGE_OUT_TIME);
    }

    private void WriteItems(PacketBitWriter writer, Map<WorldExchangeItemStatusType, List<WorldExchangeHolder>> map,
        WorldExchangeItemStatusType type)
    {
        if (map.TryGetValue(type, out List<WorldExchangeHolder>? items))
        {
            writer.WriteInt32(items.Count);
            foreach (WorldExchangeHolder holder in items)
                WriteItemInfo(writer, holder);
        }
        else
            writer.WriteInt32(0);
    }

    private void WriteItemInfo(PacketBitWriter writer, WorldExchangeHolder holder)
    {
        writer.WriteInt64(holder.getWorldExchangeId());
        writer.WriteInt64(holder.getPrice());
        writer.WriteInt32(holder.getEndTime().getEpochSecond()); // TODO can be wrong

        Item item = holder.getItemInstance();
        writer.WriteInt32(item.Id);
        writer.WriteInt64(item.getCount());
        writer.WriteInt32(item.getEnchantLevel() < 1 ? 0 : item.getEnchantLevel());

        VariationInstance? iv = item.getAugmentation();
        writer.WriteInt32(iv?.getOption1Id() ?? 0);
        writer.WriteInt32(iv?.getOption2Id() ?? 0);
        writer.WriteInt32(-1); // IntensiveItemClassID
        writer.WriteInt16((short)(item.getAttackAttribute()?.getType() ?? 0));
        writer.WriteInt16((short)(item.getAttackAttribute()?.getValue() ?? 0));
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
            writer.WriteInt32(soul[0].getId());
        }
        catch (IndexOutOfRangeException)
        {
            writer.WriteInt32(0); // TODO logging
        }

        try
        {
            writer.WriteInt32(soul[1].getId());
        }
        catch (IndexOutOfRangeException)
        {
            writer.WriteInt32(0); // TODO logging
        }

        List<EnsoulOption> specialSoul = (List<EnsoulOption>)holder.getItemInfo().getSoulCrystalSpecialOptions();
        try
        {
            writer.WriteInt32(specialSoul[0].getId());
        }
        catch (IndexOutOfRangeException)
        {
            writer.WriteInt32(0); // TODO logging
        }

        writer.WriteInt16(item.isBlessed());
    }
}