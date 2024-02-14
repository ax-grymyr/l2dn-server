using System.Xml.Linq;
using L2Dn.GameServer.Db;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class FakePlayerHolder
{
	private readonly CharacterClass _classId;
	private readonly int _hair;
	private readonly int _hairColor;
	private readonly int _face;
	private readonly int _nameColor;
	private readonly int _titleColor;
	private readonly int _equipHead;
	private readonly int _equipRHand;
	private readonly int _equipLHand;
	private readonly int _equipGloves;
	private readonly int _equipChest;
	private readonly int _equipLegs;
	private readonly int _equipFeet;
	private readonly int _equipCloak;
	private readonly int _equipHair;
	private readonly int _equipHair2;
	private readonly int _agathionId;
	private readonly int _weaponEnchantLevel;
	private readonly int _armorEnchantLevel;
	private readonly bool _fishing;
	private readonly int _baitLocationX;
	private readonly int _baitLocationY;
	private readonly int _baitLocationZ;
	private readonly int _recommends;
	private readonly int _nobleLevel;
	private readonly bool _hero;
	private readonly int _clanId;
	private readonly int _pledgeStatus;

	public FakePlayerHolder(XElement element)
	{
		_classId = (CharacterClass)element.Attribute("classId").GetInt32(1);
		_hair = element.Attribute("hair").GetInt32(1);
		_hairColor = element.Attribute("hairColor").GetInt32(1);
		_face = element.Attribute("face").GetInt32(1);
		_nameColor = element.Attribute("nameColor").GetInt32(0xFFFFFF);
		_titleColor = element.Attribute("titleColor").GetInt32(0xECF9A2);
		_equipHead = element.Attribute("equipHead").GetInt32(0);
		_equipRHand = element.Attribute("equipRHand").GetInt32(0); // or dual hand
		_equipLHand = element.Attribute("equipLHand").GetInt32(0);
		_equipGloves = element.Attribute("equipGloves").GetInt32(0);
		_equipChest = element.Attribute("equipChest").GetInt32(0);
		_equipLegs = element.Attribute("equipLegs").GetInt32(0);
		_equipFeet = element.Attribute("equipFeet").GetInt32(0);
		_equipCloak = element.Attribute("equipCloak").GetInt32(0);
		_equipHair = element.Attribute("equipHair").GetInt32(0);
		_equipHair2 = element.Attribute("equipHair2").GetInt32(0);
		_agathionId = element.Attribute("agathionId").GetInt32(0);
		_weaponEnchantLevel = element.Attribute("weaponEnchantLevel").GetInt32(0);
		_armorEnchantLevel = element.Attribute("armorEnchantLevel").GetInt32(0);
		_fishing = element.Attribute("fishing").GetBoolean(false);
		_baitLocationX = element.Attribute("baitLocationX").GetInt32(0);
		_baitLocationY = element.Attribute("baitLocationY").GetInt32(0);
		_baitLocationZ = element.Attribute("baitLocationZ").GetInt32(0);
		_recommends = element.Attribute("recommends").GetInt32(0);
		_nobleLevel = element.Attribute("nobleLevel").GetInt32(0);
		_hero = element.Attribute("hero").GetBoolean(false);
		_clanId = element.Attribute("clanId").GetInt32(0);
		_pledgeStatus = element.Attribute("pledgeStatus").GetInt32(0);
	}

	public CharacterClass getClassId()
	{
		return _classId;
	}

	public int getHair()
	{
		return _hair;
	}

	public int getHairColor()
	{
		return _hairColor;
	}

	public int getFace()
	{
		return _face;
	}

	public int getNameColor()
	{
		return _nameColor;
	}

	public int getTitleColor()
	{
		return _titleColor;
	}

	public int getEquipHead()
	{
		return _equipHead;
	}

	public int getEquipRHand()
	{
		return _equipRHand;
	}

	public int getEquipLHand()
	{
		return _equipLHand;
	}

	public int getEquipGloves()
	{
		return _equipGloves;
	}

	public int getEquipChest()
	{
		return _equipChest;
	}

	public int getEquipLegs()
	{
		return _equipLegs;
	}

	public int getEquipFeet()
	{
		return _equipFeet;
	}

	public int getEquipCloak()
	{
		return _equipCloak;
	}

	public int getEquipHair()
	{
		return _equipHair;
	}

	public int getEquipHair2()
	{
		return _equipHair2;
	}

	public int getAgathionId()
	{
		return _agathionId;
	}

	public int getWeaponEnchantLevel()
	{
		return _weaponEnchantLevel;
	}

	public int getArmorEnchantLevel()
	{
		return _armorEnchantLevel;
	}

	public bool isFishing()
	{
		return _fishing;
	}

	public int getBaitLocationX()
	{
		return _baitLocationX;
	}

	public int getBaitLocationY()
	{
		return _baitLocationY;
	}

	public int getBaitLocationZ()
	{
		return _baitLocationZ;
	}

	public int getRecommends()
	{
		return _recommends;
	}

	public int getNobleLevel()
	{
		return _nobleLevel;
	}

	public bool isHero()
	{
		return _hero;
	}

	public int getClanId()
	{
		return _clanId;
	}

	public int getPledgeStatus()
	{
		return _pledgeStatus;
	}
}