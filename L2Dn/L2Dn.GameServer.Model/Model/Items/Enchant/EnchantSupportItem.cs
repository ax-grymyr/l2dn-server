using L2Dn.GameServer.Model.Items.Types;
using L2Dn.Model.Xml;

namespace L2Dn.GameServer.Model.Items.Enchant;

/**
 * @author UnAfraid
 */
public class EnchantSupportItem: AbstractEnchantItem
{
	private readonly bool _isWeapon;
	private readonly bool _isBlessed;
	private readonly bool _isGiant;

	public EnchantSupportItem(XmlEnchantScroll xmlEnchantScroll, ItemType type): base(xmlEnchantScroll)
	{
		_isWeapon = type == EtcItemType.ENCHT_ATTR_INC_PROP_ENCHT_WP ||
			type == EtcItemType.BLESSED_ENCHT_ATTR_INC_PROP_ENCHT_WP ||
			type == EtcItemType.GIANT_ENCHT_ATTR_INC_PROP_ENCHT_WP ||
			type == EtcItemType.BLESSED_GIANT_ENCHT_ATTR_INC_PROP_ENCHT_WP;

		_isBlessed = type == EtcItemType.BLESSED_ENCHT_ATTR_INC_PROP_ENCHT_AM ||
			type == EtcItemType.BLESSED_ENCHT_ATTR_INC_PROP_ENCHT_WP ||
			type == EtcItemType.BLESSED_GIANT_ENCHT_ATTR_INC_PROP_ENCHT_AM ||
			type == EtcItemType.BLESSED_GIANT_ENCHT_ATTR_INC_PROP_ENCHT_WP;

		_isGiant = type == EtcItemType.GIANT_ENCHT_ATTR_INC_PROP_ENCHT_AM ||
			type == EtcItemType.GIANT_ENCHT_ATTR_INC_PROP_ENCHT_WP ||
			type == EtcItemType.BLESSED_GIANT_ENCHT_ATTR_INC_PROP_ENCHT_AM ||
			type == EtcItemType.BLESSED_GIANT_ENCHT_ATTR_INC_PROP_ENCHT_WP;
	}

	public override bool isWeapon()
	{
		return _isWeapon;
	}

	public bool isBlessed()
	{
		return _isBlessed;
	}

	public bool isGiant()
	{
		return _isGiant;
	}
}