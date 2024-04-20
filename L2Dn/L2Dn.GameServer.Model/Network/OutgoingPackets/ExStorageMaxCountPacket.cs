using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExStorageMaxCountPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _inventory;
    private readonly int _warehouse;
    // private int _freight; // Removed with 152.
    private readonly int _clan;
    private readonly int _privateSell;
    private readonly int _privateBuy;
    private readonly int _receipeD;
    private readonly int _recipe;
    private readonly int _inventoryExtraSlots;
    private readonly int _inventoryQuestItems;
	
    public ExStorageMaxCountPacket(Player player)
    {
        if (!player.isSubclassLocked()) // Changing class. // TODO: logic must be in model
        {
            _player = player;
            _inventory = player.getInventoryLimit();
            _warehouse = player.getWareHouseLimit();
            // _freight = Config.ALT_FREIGHT_SLOTS; // Removed with 152.
            _privateSell = player.getPrivateSellStoreLimit();
            _privateBuy = player.getPrivateBuyStoreLimit();
            _clan = Config.WAREHOUSE_SLOTS_CLAN;
            _receipeD = player.getDwarfRecipeLimit();
            _recipe = player.getCommonRecipeLimit();
            _inventoryExtraSlots = (int) player.getStat().getValue(Stat.INVENTORY_NORMAL, 0);
            _inventoryQuestItems = Config.INVENTORY_MAXIMUM_QUEST_ITEMS;
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        if (_player == null)
        {
            return;
        }
		
        writer.WritePacketCode(OutgoingPacketCodes.EX_STORAGE_MAX_COUNT);
        
        writer.WriteInt32(_inventory);
        writer.WriteInt32(_warehouse);
        // writer.WriteInt32(_freight); // Removed with 152.
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