using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Model.Interfaces;

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

	protected ImmutableArray<MultisellEntryHolder> _entries;
	private readonly FrozenSet<int> _npcsAllowed;

	public MultisellListHolder(int listId, bool isChanceMultisell, bool applyTaxes, bool maintainEnchantment,
		double ingredientMultiplier, double productMultiplier, ImmutableArray<MultisellEntryHolder> entries, 
		FrozenSet<int> npcsAllowed)
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

	public ImmutableArray<MultisellEntryHolder> getEntries()
	{
		return _entries;
	}

	public FrozenSet<int> getNpcsAllowed()
	{
		return _npcsAllowed;
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
		return _npcsAllowed != null && _npcsAllowed.Contains(npcId);
	}
}