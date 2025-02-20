using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExStorageMaxCountPacket: IOutgoingPacket
{
    private readonly int _inventory;
    private readonly int _warehouse;
    private readonly int _clan;
    private readonly int _privateSell;
    private readonly int _privateBuy;
    private readonly int _receipeD;
    private readonly int _recipe;
    private readonly int _inventoryExtraSlots;
    private readonly int _inventoryQuestItems;

    public ExStorageMaxCountPacket(Player player)
    {
        _inventory = player.getInventoryLimit();
        _warehouse = player.getWareHouseLimit();
        _privateSell = player.getPrivateSellStoreLimit();
        _privateBuy = player.getPrivateBuyStoreLimit();
        _clan = Config.WAREHOUSE_SLOTS_CLAN;
        _receipeD = player.getDwarfRecipeLimit();
        _recipe = player.getCommonRecipeLimit();
        _inventoryExtraSlots = (int) player.getStat().getValue(Stat.INVENTORY_NORMAL, 0);
        _inventoryQuestItems = Config.INVENTORY_MAXIMUM_QUEST_ITEMS;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_STORAGE_MAX_COUNT);

        writer.WriteInt32(_inventory);
        writer.WriteInt32(_warehouse);
        writer.WriteInt32(_clan);
        writer.WriteInt32(_privateSell);
        writer.WriteInt32(_privateBuy);
        writer.WriteInt32(_receipeD);
        writer.WriteInt32(_recipe);
        writer.WriteInt32(_inventoryExtraSlots); // Belt inventory slots increase count
        writer.WriteInt32(_inventoryQuestItems);
        writer.WriteInt32(40); // TODO: Find me!
        writer.WriteInt32(40); // TODO: Find me!
        writer.WriteInt32(0x64); // Artifact slots (Fixed)
    }
}