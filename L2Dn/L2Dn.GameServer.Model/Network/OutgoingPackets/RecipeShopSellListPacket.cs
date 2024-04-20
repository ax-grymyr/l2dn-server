using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct RecipeShopSellListPacket: IOutgoingPacket
{
    private readonly Player _buyer;
    private readonly Player _manufacturer;
    private readonly double _craftRate;
    private readonly double _craftCritical;
	
    public RecipeShopSellListPacket(Player buyer, Player manufacturer)
    {
        _buyer = buyer;
        _manufacturer = manufacturer;
        _craftRate = _manufacturer.getStat().getValue(Stat.CRAFT_RATE, 0);
        _craftCritical = _manufacturer.getStat().getValue(Stat.CRAFTING_CRITICAL, 0);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.RECIPE_SHOP_SELL_LIST);
        
        writer.WriteInt32(_manufacturer.getObjectId());
        writer.WriteInt32((int) _manufacturer.getCurrentMp()); // Creator's MP
        writer.WriteInt32(_manufacturer.getMaxMp()); // Creator's MP
        writer.WriteInt64(_buyer.getAdena()); // Buyer Adena
        if (!_manufacturer.hasManufactureShop())
        {
            writer.WriteInt32(0);
        }
        else
        {
            writer.WriteInt32(_manufacturer.getManufactureItems().size());
            foreach (ManufactureItem item in _manufacturer.getManufactureItems().values())
            {
                writer.WriteInt32(item.getRecipeId());
                writer.WriteInt32(0); // CanCreate?
                writer.WriteInt64(item.getCost());
                writer.WriteDouble(Math.Min(_craftRate, 100.0));
                writer.WriteByte(_craftCritical > 0);
                writer.WriteDouble(Math.Min(_craftCritical, 100.0));
            }
        }
    }
}