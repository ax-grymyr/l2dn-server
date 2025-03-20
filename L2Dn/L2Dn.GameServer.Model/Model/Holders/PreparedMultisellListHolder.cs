using System.Collections.Immutable;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Holders;

/**
 * A modified version of {@link MultisellListHolder} that may include altered data of the original and other dynamic data resulted from players' interraction.
 * @author Nik
 */
public class PreparedMultisellListHolder: MultisellListHolder
{
	private int _npcObjectId;
	private readonly bool _inventoryOnly;
	private double _taxRate;
	private List<ItemInfo>? _itemInfos;

	public PreparedMultisellListHolder(MultisellListHolder list, bool inventoryOnly, ItemContainer inventory, Npc? npc,
		double ingredientMultiplier, double productMultiplier)
		: base(list.Id, list.isChanceMultisell(), list.isApplyTaxes(), list.isMaintainEnchantment(),
			list.getIngredientMultiplier(), list.getProductMultiplier(), list.getEntries(), list.getNpcsAllowed())
	{
		_inventoryOnly = inventoryOnly;
		if (npc != null)
		{
			_npcObjectId = npc.ObjectId;
			_taxRate = npc.getCastleTaxRate(TaxType.BUY);
		}

		// Display items from inventory that are available for exchange.
		if (inventoryOnly)
		{
			_entries = ImmutableArray<MultisellEntryHolder>.Empty;
			_itemInfos = new();

			// Only do the match up on equippable items that are not currently equipped. For each appropriate item, produce a set of entries for the multisell list.
			foreach (Item item in inventory.getItems())
			{
				if (!item.isEquipped() && (item.isArmor() || item.isWeapon()))
				{
					// Check ingredients of each entry to see if it's an entry we'd like to include.
					foreach (MultisellEntryHolder entry in list.getEntries())
					{
						foreach (ItemChanceHolder holder in entry.getIngredients())
						{
							if (holder.Id == item.Id)
							{
								_entries = _entries.Add(entry);
								_itemInfos.Add(new ItemInfo(item));
							}
						}
					}
				}
			}
		}
	}

	public ItemInfo? getItemEnchantment(int index)
	{
		return _itemInfos != null ? _itemInfos[index] : null;
	}

	public double getTaxRate()
	{
		return isApplyTaxes() ? _taxRate : 0;
	}

	public bool isInventoryOnly()
	{
		return _inventoryOnly;
	}

	public bool checkNpcObjectId(int npcObjectId)
	{
		return _npcObjectId == 0 || _npcObjectId == npcObjectId;
	}

	/**
	 * @param ingredient
	 * @return the new count of the given ingredient after applying ingredient multiplier and adena tax rate.
	 */
	public long getIngredientCount(ItemHolder ingredient)
	{
		return (long)(ingredient.Id == Inventory.ADENA_ID
			? Math.Round(ingredient.getCount() * getIngredientMultiplier() * (1 + getTaxRate()))
			: Math.Round(ingredient.getCount() * getIngredientMultiplier()));
	}

	/**
	 * @param product
	 * @return the new count of the given product after applying product multiplier.
	 */
	public long getProductCount(ItemChanceHolder product)
	{
		return (long)Math.Round(product.getCount() * getProductMultiplier());
	}
}