using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Types;

namespace L2Dn.GameServer.Model.Items;

/**
* This class is dedicated to the management of EtcItem.
*/
public class EtcItem: ItemTemplate
{
    private readonly EtcItemType _type;
	private readonly string _handler;
	private List<ExtractableProduct> _extractableItems = [];
	private readonly int _extractableCountMin;
	private readonly int _extractableCountMax;
	private readonly bool _isInfinite;
	private bool _isMineral;
	private bool _isEnsoulStone;

	/**
 * Constructor for EtcItem.
 * @param set StatSet designating the set of couples (key,value) for description of the Etc
 */
	public EtcItem(StatSet set): base(set)
	{
        _type = set.getEnum("etcitem_type", EtcItemType.NONE);
        _type1 = TYPE1_ITEM_QUESTITEM_ADENA;
        _type2 = TYPE2_OTHER; // default is other

        if (isQuestItem())
        {
            _type2 = TYPE2_QUEST;
        }
        else if (Id == Inventory.AdenaId || Id == Inventory.ANCIENT_ADENA_ID)
        {
            _type2 = TYPE2_MONEY;
        }

        _handler = set.getString("handler", string.Empty); // ! null !

        _extractableCountMin = set.getInt("extractableCountMin", 0);
        _extractableCountMax = set.getInt("extractableCountMax", 0);
        if (_extractableCountMin > _extractableCountMax)
        {
            LOGGER.Warn("Item " + this + " extractableCountMin is bigger than extractableCountMax!");
        }

        _isInfinite = set.getBoolean("is_infinite", false);
	}

	public override ItemType getItemType()
	{
		return _type;
	}

	/**
 * @return the handler name, null if no handler for item.
 */
	public string getHandlerName()
	{
		return _handler;
	}

	/**
 * @return the extractable items list.
 */
	public List<ExtractableProduct> getExtractableItems()
	{
		return _extractableItems;
	}

	/**
 * @return the minimum count of extractable items
 */
	public int getExtractableCountMin()
	{
		return _extractableCountMin;
	}

	/**
 * @return the maximum count of extractable items
 */
	public int getExtractableCountMax()
	{
		return _extractableCountMax;
	}

	/**
 * @return true if item is infinite
 */
	public bool isInfinite()
	{
		return _isInfinite;
	}

	/**
 * @param extractableProduct
 */
	public override void addCapsuledItem(ExtractableProduct extractableProduct)
	{
		if (_extractableItems == null)
		{
			_extractableItems = new();
		}

		_extractableItems.Add(extractableProduct);
	}

	public bool isMineral()
	{
		return _isMineral;
	}

	public void setMineral()
	{
		_isMineral = true;
	}

	public bool isEnsoulStone()
	{
		return _isEnsoulStone;
	}

	public void setEnsoulStone()
	{
		_isEnsoulStone = true;
	}
}