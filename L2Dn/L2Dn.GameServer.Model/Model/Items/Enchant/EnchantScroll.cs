using System.Collections.Frozen;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Model.Xml;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Items.Enchant;

/**
 * @author UnAfraid, Mobius
 */
public class EnchantScroll: AbstractEnchantItem
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(EnchantScroll));

	private readonly bool _isWeapon;
	private readonly bool _isBlessed;
	private readonly bool _isBlessedDown;
	private readonly bool _isSafe;
	private readonly bool _isGiant;
	private readonly bool _isCursed;
	private readonly int _scrollGroupId;
	private readonly FrozenDictionary<int, int> _items;

	public EnchantScroll(XmlEnchantScroll enchantScroll, ItemType type, FrozenDictionary<int, int> items): base(
		enchantScroll)
	{
		_scrollGroupId = enchantScroll.ScrollGroupId;
		_items = items.ToFrozenDictionary();

		_isWeapon = type == EtcItemType.ENCHT_ATTR_ANCIENT_CRYSTAL_ENCHANT_WP || type == EtcItemType.BLESS_ENCHT_WP ||
			type == EtcItemType.ENCHT_WP || type == EtcItemType.GIANT_ENCHT_WP || type == EtcItemType.CURSED_ENCHT_WP;

		_isBlessed = type == EtcItemType.BLESS_ENCHT_AM || type == EtcItemType.BLESS_ENCHT_WP ||
			type == EtcItemType.BLESSED_ENCHT_ATTR_INC_PROP_ENCHT_WP ||
			type == EtcItemType.BLESSED_ENCHT_ATTR_INC_PROP_ENCHT_AM ||
			type == EtcItemType.BLESSED_GIANT_ENCHT_ATTR_INC_PROP_ENCHT_AM ||
			type == EtcItemType.BLESSED_GIANT_ENCHT_ATTR_INC_PROP_ENCHT_WP;

		_isBlessedDown = type == EtcItemType.BLESS_ENCHT_AM_DOWN;
		_isSafe = type == EtcItemType.ENCHT_ATTR_ANCIENT_CRYSTAL_ENCHANT_AM ||
			type == EtcItemType.ENCHT_ATTR_ANCIENT_CRYSTAL_ENCHANT_WP ||
			type == EtcItemType.ENCHT_ATTR_CRYSTAL_ENCHANT_AM || type == EtcItemType.ENCHT_ATTR_CRYSTAL_ENCHANT_WP;

		_isGiant = type == EtcItemType.GIANT_ENCHT_AM || type == EtcItemType.GIANT_ENCHT_WP;
		_isCursed = type == EtcItemType.CURSED_ENCHT_AM || type == EtcItemType.CURSED_ENCHT_WP;
	}

	public override bool isWeapon()
	{
		return _isWeapon;
	}

	/**
	 * @return {@code true} for blessed scrolls (enchanted item will remain on failure and enchant value will reset to 0), {@code false} otherwise
	 */
	public bool isBlessed()
	{
		return _isBlessed;
	}

	/**
	 * @return {@code true} for blessed scrolls (enchanted item will remain on failure and enchant value will go down by 1), {@code false} otherwise
	 */
	public bool isBlessedDown()
	{
		return _isBlessedDown;
	}

	/**
	 * @return {@code true} for safe-enchant scrolls (enchant level will remain on failure), {@code false} otherwise
	 */
	public bool isSafe()
	{
		return _isSafe;
	}

	public bool isGiant()
	{
		return _isGiant;
	}

	public bool isCursed()
	{
		return _isCursed;
	}

	public ICollection<int> getItems()
	{
		return _items.Keys;
	}

	/**
	 * @param itemToEnchant the item to be enchanted
	 * @param supportItem the support item used when enchanting (can be null)
	 * @return {@code true} if this scroll can be used with the specified support item and the item to be enchanted, {@code false} otherwise
	 */
	public override bool isValid(Item itemToEnchant, EnchantSupportItem? supportItem)
	{
		if (_items.Count != 0 && !_items.ContainsKey(itemToEnchant.getId()))
		{
			return false;
		}

		if (supportItem != null)
		{
			if ((isBlessed() && !supportItem.isBlessed()) || (!isBlessed() && supportItem.isBlessed()))
			{
				return false;
			}

			if ((isBlessedDown() && !supportItem.isBlessed()) || (!isBlessedDown() && supportItem.isBlessed()))
			{
				return false;
			}
			if ((isGiant() && !supportItem.isGiant()) || (!isGiant() && supportItem.isGiant()))
			{
				return false;
			}
			if (!supportItem.isValid(itemToEnchant, supportItem))
			{
				return false;
			}
			if (supportItem.isWeapon() != isWeapon())
			{
				return false;
			}
		}

		if (_items.Count == 0)
		{
			if (isActionBlessed() && itemToEnchant.isWeapon() &&
			    itemToEnchant.getTemplate().getCrystalType() == getGrade())
			{
				return true;
			}

			foreach (EnchantScroll scroll in EnchantItemData.getInstance().getScrolls())
			{
				if (scroll.getId() == getId())
				{
					continue;
				}

				ICollection<int> scrollItems = scroll.getItems();
				if (scrollItems.Count == 0 && scrollItems.Contains(itemToEnchant.getId()))
				{
					return false;
				}
			}
		}

		return base.isValid(itemToEnchant, supportItem);
	}

	/**
	 * @param player
	 * @param enchantItem
	 * @return the chance of current scroll's group.
	 */
	public double getChance(Player player, Item enchantItem)
	{
		int scrollGroupId = _items.GetValueOrDefault(enchantItem.getId(), _scrollGroupId);
		if (EnchantItemGroupsData.getInstance().getScrollGroup(scrollGroupId) == null)
		{
			_logger.Warn(GetType().Name + ": Unexistent enchant scroll group specified for enchant scroll: " + getId());
			return -1;
		}

		EnchantItemGroup? group = EnchantItemGroupsData.getInstance()
			.getItemGroup(enchantItem.getTemplate(), scrollGroupId);

		if (group == null)
		{
			_logger.Warn(GetType().Name + ": Couldn't find enchant item group for scroll: " + getId() +
				" requested by: " + player);
			return -1;
		}

		if (getSafeEnchant() > 0 && enchantItem.getEnchantLevel() < getSafeEnchant())
		{
			return 100;
		}

		return group.getChance(enchantItem.getEnchantLevel());
	}

	/**
	 * @param player
	 * @param enchantItem
	 * @param supportItem
	 * @return the total chance for success rate of this scroll
	 */
	public EnchantResultType calculateSuccess(Player player, Item enchantItem, EnchantSupportItem? supportItem)
	{
		if (!isValid(enchantItem, supportItem))
		{
			return EnchantResultType.ERROR;
		}

		double chance = getChance(player, enchantItem);
		if (chance == -1)
		{
			return EnchantResultType.ERROR;
		}

		CrystalType crystalLevel = enchantItem.getTemplate().getCrystalType().getLevel();
		double enchantRateStat =
			crystalLevel > CrystalType.NONE.getLevel() && crystalLevel < CrystalType.EVENT.getLevel()
				? player.getStat().getValue(Stat.ENCHANT_RATE)
				: 0;

		double bonusRate = getBonusRate();
		double supportBonusRate = supportItem != null ? supportItem.getBonusRate() : 0;
		double finalChance = Math.Min(chance + bonusRate + supportBonusRate + enchantRateStat, 100);
		double random = 100 * Rnd.nextDouble();
		bool success = random < finalChance;
		return success ? EnchantResultType.SUCCESS : EnchantResultType.FAILURE;
	}
}