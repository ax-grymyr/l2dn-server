using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct RecipeBookItemListPacket: IOutgoingPacket
{
    private readonly ICollection<RecipeList> _recipes;
    private readonly bool _isDwarvenCraft;
    private readonly int _maxMp;
	
    public RecipeBookItemListPacket(ICollection<RecipeList> recipeBook, bool isDwarvenCraft, int maxMp)
    {
        _recipes = recipeBook;
        _isDwarvenCraft = isDwarvenCraft;
        _maxMp = maxMp;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.RECIPE_BOOK_ITEM_LIST);
        writer.WriteInt32(!_isDwarvenCraft); // 0 = Dwarven - 1 = Common
        writer.WriteInt32(_maxMp);
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
    }
}