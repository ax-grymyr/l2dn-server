using L2Dn.GameServer.Data.Xml;

namespace L2Dn.GameServer.Model;

public class ManufactureItem
{
    private readonly int _recipeId;
    private readonly long _cost;
    private readonly bool _isDwarven;

    public ManufactureItem(int recipeId, long cost)
    {
        RecipeList recipeList = RecipeData.getInstance().getRecipeList(recipeId) ??
            throw new ArgumentException("Invalid recipeId");

        _recipeId = recipeId;
        _cost = cost;
        _isDwarven = recipeList.isDwarvenRecipe();
    }

    public int getRecipeId()
    {
        return _recipeId;
    }

    public long getCost()
    {
        return _cost;
    }

    public bool isDwarven()
    {
        return _isDwarven;
    }
}