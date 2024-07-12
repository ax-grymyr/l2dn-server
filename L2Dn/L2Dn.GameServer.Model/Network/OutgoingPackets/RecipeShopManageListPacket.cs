using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct RecipeShopManageListPacket: IOutgoingPacket
{
    private readonly Player _seller;
    private readonly bool _isDwarven;
    private readonly ICollection<RecipeList> _recipes;
	
    public RecipeShopManageListPacket(Player seller, bool isDwarven)
    {
        _seller = seller;
        _isDwarven = isDwarven;
        if (_isDwarven && _seller.hasDwarvenCraft())
        {
            _recipes = _seller.getDwarvenRecipeBook();
        }
        else
        {
            _recipes = _seller.getCommonRecipeBook();
        }

        // TODO: must be somewhere else, but not in the packet
        // if (_seller.hasManufactureShop())
        // {
        //     Iterator<ManufactureItem> it = _seller.getManufactureItems().values().iterator();
        //     ManufactureItem item;
        //     while (it.hasNext())
        //     {
        //         item = it.next();
        //         if ((item.isDwarven() != _isDwarven) || !seller.hasRecipeList(item.getRecipeId()))
        //         {
        //             it.remove();
        //         }
        //     }
        // }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.RECIPE_SHOP_MANAGE_LIST);
        
        writer.WriteInt32(_seller.getObjectId());
        writer.WriteInt32((int) _seller.getAdena());
        writer.WriteInt32(!_isDwarven);
        if (_recipes == null)
        {
            writer.WriteInt32(0);
        }
        else
        {
            writer.WriteInt32(_recipes.Count); // number of items in recipe book
            int count = 1;
            foreach (RecipeList recipe in _recipes)
            {
                writer.WriteInt32(recipe.getId());
                writer.WriteInt32(count++);
            }
        }
        if (!_seller.hasManufactureShop())
        {
            writer.WriteInt32(0);
        }
        else
        {
            writer.WriteInt32(_seller.getManufactureItems().size());
            foreach (ManufactureItem item in _seller.getManufactureItems().Values)
            {
                writer.WriteInt32(item.getRecipeId());
                writer.WriteInt32(0);
                writer.WriteInt64(item.getCost());
            }
        }
    }
}