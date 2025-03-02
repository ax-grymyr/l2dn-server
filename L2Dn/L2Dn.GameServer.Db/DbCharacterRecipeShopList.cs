using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(RecipeId))]
public class DbCharacterRecipeShopList
{
    public int CharacterId { get; set; }
    public int RecipeId { get; set; }
    public long Price { get; set; }
    public short Index { get; set; }
}