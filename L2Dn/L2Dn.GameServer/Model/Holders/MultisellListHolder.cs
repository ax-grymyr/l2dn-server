using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Holders;

/**
 * A static list container of all multisell entries of a given list.
 * @author Nik
 */
public class MultisellListHolder: IIdentifiable
{
	private readonly int _listId;
	private readonly bool _isChanceMultisell;
	private readonly bool _applyTaxes;
	private readonly bool _maintainEnchantment;
	private readonly double _ingredientMultiplier;
	private readonly double _productMultiplier;

	protected List<MultisellEntryHolder> _entries;
	protected readonly Set<int> _npcsAllowed;

	public MultisellListHolder(int listId, bool isChanceMultisell, bool applyTaxes, bool maintainEnchantment,
		double ingredientMultiplier, double productMultiplier, List<MultisellEntryHolder> entries, Set<int> npcsAllowed)
	{
		_listId = listId;
		_isChanceMultisell = isChanceMultisell;
		_applyTaxes = applyTaxes;
		_maintainEnchantment = maintainEnchantment;
		_ingredientMultiplier = ingredientMultiplier;
		_productMultiplier = productMultiplier;
		_entries = entries;
		_npcsAllowed = npcsAllowed;
	}

	public MultisellListHolder(StatSet set)
	{
		_listId = set.getInt("listId");
		_isChanceMultisell = set.getBoolean("isChanceMultisell", false);
		_applyTaxes = set.getBoolean("applyTaxes", false);
		_maintainEnchantment = set.getBoolean("maintainEnchantment", false);
		_ingredientMultiplier = set.getDouble("ingredientMultiplier", 1.0);
		_productMultiplier = set.getDouble("productMultiplier", 1.0);
		_entries = Collections.unmodifiableList(set.getList<MultisellEntryHolder>("entries", Collections.emptyList()));
		_npcsAllowed = set.getObject<Set<int>>("allowNpc");
	}

	public List<MultisellEntryHolder> getEntries()
	{
		return _entries;
	}

	public int getId()
	{
		return _listId;
	}

	public bool isChanceMultisell()
	{
		return _isChanceMultisell;
	}

	public bool isApplyTaxes()
	{
		return _applyTaxes;
	}

	public bool isMaintainEnchantment()
	{
		return _maintainEnchantment;
	}

	public double getIngredientMultiplier()
	{
		return _ingredientMultiplier;
	}

	public double getProductMultiplier()
	{
		return _productMultiplier;
	}

	public bool isNpcAllowed(int npcId)
	{
		return (_npcsAllowed != null) && _npcsAllowed.contains(npcId);
	}
}