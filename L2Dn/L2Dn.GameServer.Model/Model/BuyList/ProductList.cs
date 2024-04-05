using System.Collections.Immutable;

namespace L2Dn.GameServer.Model.BuyList;

public class ProductList
{
    private readonly int _listId;
    private readonly ImmutableArray<Product> _products;
    private readonly ImmutableDictionary<int, Product> _productsByIds;
    private readonly ImmutableSortedSet<int> _allowedNpcs;
	
    public ProductList(int listId, ImmutableArray<Product> products, ImmutableSortedSet<int> allowedNpcs)
    {
        _listId = listId;
        _products = products;
        _productsByIds = products.ToImmutableDictionary(x => x.getItemId());
        _allowedNpcs = allowedNpcs;
    }
	
    public int getListId()
    {
        return _listId;
    }
	
    public ImmutableArray<Product> getProducts()
    {
        return _products;
    }
	
    public Product? getProductByItemId(int itemId)
    {
        return _productsByIds.GetValueOrDefault(itemId);
    }
	
    public bool isNpcAllowed(int npcId)
    {
        return _allowedNpcs.Contains(npcId);
    }
	
    public ImmutableSortedSet<int> getNpcsAllowed()
    {
        return _allowedNpcs;
    }
}
