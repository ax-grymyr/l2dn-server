using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.BuyList;

public class ProductList
{
    private readonly int _listId;
    private readonly  Map<int, Product> _products = new();
    private Set<int> _allowedNpcs = null;
	
    public ProductList(int listId)
    {
        _listId = listId;
    }
	
    public int getListId()
    {
        return _listId;
    }
	
    public ICollection<Product> getProducts()
    {
        return _products.values();
    }
	
    public Product getProductByItemId(int itemId)
    {
        return _products.get(itemId);
    }
	
    public void addProduct(Product product)
    {
        _products.put(product.getItemId(), product);
    }
	
    public void addAllowedNpc(int npcId)
    {
        if (_allowedNpcs == null)
        {
            _allowedNpcs = new();
        }
        _allowedNpcs.add(npcId);
    }
	
    public bool isNpcAllowed(int npcId)
    {
        return (_allowedNpcs != null) && _allowedNpcs.contains(npcId);
    }
	
    public Set<int> getNpcsAllowed()
    {
        return _allowedNpcs;
    }
}
