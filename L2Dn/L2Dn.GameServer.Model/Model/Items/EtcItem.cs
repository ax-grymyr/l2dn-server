using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Types;

namespace L2Dn.GameServer.Model.Items;

/**
* This class is dedicated to the management of EtcItem.
*/
public class EtcItem: ItemTemplate
{
	private string _handler = string.Empty;
	private EtcItemType _type;
	private List<ExtractableProduct> _extractableItems = [];
	private int _extractableCountMin;
	private int _extractableCountMax;
	private bool _isInfinite;
	private bool _isMineral;
	private bool _isEnsoulStone;

	/**
 * Constructor for EtcItem.
 * @param set StatSet designating the set of couples (key,value) for description of the Etc
 */
	public EtcItem(StatSet set): base(set)
	{
	}

	public override void set(StatSet set)
	{
		base.set(set);
		_type = set.getEnum("etcitem_type", EtcItemType.NONE);
		_type1 = ItemTemplate.TYPE1_ITEM_QUESTITEM_ADENA;
		_type2 = ItemTemplate.TYPE2_OTHER; // default is other

		if (isQuestItem())
		{
			_type2 = ItemTemplate.TYPE2_QUEST;
		}
		else if (getId() == Inventory.ADENA_ID || getId() == Inventory.ANCIENT_ADENA_ID)
		{
			_type2 = ItemTemplate.TYPE2_MONEY;
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