using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct RecipeShopItemInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _recipeId;
    private readonly double _craftRate;
    private readonly double _craftCritical;
	
    public RecipeShopItemInfoPacket(Player player, int recipeId)
    {
        _player = player;
        _recipeId = recipeId;
        _craftRate = _player.getStat().getValue(Stat.CRAFT_RATE, 0);
        _craftCritical = _player.getStat().getValue(Stat.CRAFTING_CRITICAL, 0);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.RECIPE_SHOP_ITEM_INFO);
        writer.WriteInt32(_player.getObjectId());
        writer.WriteInt32(_recipeId);
        writer.WriteInt32((int)_player.getCurrentMp());
        writer.WriteInt32(_player.getMaxMp());
        writer.WriteInt32(-1); // item creation none/success/failed
        writer.WriteInt64(0); // manufacturePrice
        writer.WriteByte(0); // Trigger offering window if 1
        writer.WriteInt64(0); // Adena worth of items for maximum offering.
        writer.WriteDouble(Math.Min(_craftRate, 100.0));
        writer.WriteByte(_craftCritical > 0);
        writer.WriteDouble(Math.Min(_craftCritical, 100.0));
        writer.WriteByte(0); // find me
    }
}