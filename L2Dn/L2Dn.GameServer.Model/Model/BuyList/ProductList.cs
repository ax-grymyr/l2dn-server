using System.Collections.Frozen;
using System.Collections.Immutable;

namespace L2Dn.GameServer.Model.BuyList;

public class ProductList
{
    private readonly int _listId;
    private readonly ImmutableArray<Product> _products;
    private readonly FrozenDictionary<int, Product> _productsByIds;
    private readonly FrozenSet<int> _allowedNpcs;
	
    public ProductList(int listId, IReadOnlyList<Product> products, IReadOnlyList<int> allowedNpcs)
    {
        _listId = listId;
        _products = products.ToImmutableArray();
        _productsByIds = products.ToFrozenDictionary(x => x.getItemId());
        _allowedNpcs = allowedNpcs.ToFrozenSet();
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
	
    public FrozenSet<int> getNpcsAllowed()
    {
        return _allowedNpcs;
    }
}
