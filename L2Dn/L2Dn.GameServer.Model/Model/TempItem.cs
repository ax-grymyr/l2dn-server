using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model;

/**
 * Class explanation:<br>
 * For item counting or checking purposes. When you don't want to modify inventory<br>
 * class contains itemId, quantity, ownerId, referencePrice, but not objectId<br>
 * is stored, this will be only "list" of items with it's owner
 */
public class TempItem
{
    private readonly int _itemId;
    private int _quantity;
    private readonly long _referencePrice;
    private readonly string _itemName;
	
    /**
     * @param item
     * @param quantity of that item
     */
    public TempItem(Item item, int quantity)
    {
        _itemId = item.getId();
        _quantity = quantity;
        _itemName = item.getTemplate().getName();
        _referencePrice = item.getReferencePrice();
    }
	
    /**
     * @return the quantity.
     */
    public int getQuantity()
    {
        return _quantity;
    }
	
    /**
     * @param quantity The quantity to set.
     */
    public void setQuantity(int quantity)
    {
        _quantity = quantity;
    }
	
    public long getReferencePrice()
    {
        return _referencePrice;
    }
	
    /**
     * @return the itemId.
     */
    public int getItemId()
    {
        return _itemId;
    }
	
    /**
     * @return the itemName.
     */
    public String getItemName()
    {
        return _itemName;
    }
}
