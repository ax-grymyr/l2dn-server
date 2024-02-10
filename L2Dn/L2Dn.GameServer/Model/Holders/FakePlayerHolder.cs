using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class FakePlayerHolder
{
	private readonly ClassId _classId;
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

	public FakePlayerHolder(StatSet set)
	{
		_classId = ClassId.getClassId(set.getInt("classId", 1));
		_hair = set.getInt("hair", 1);
		_hairColor = set.getInt("hairColor", 1);
		_face = set.getInt("face", 1);
		_nameColor = set.getInt("nameColor", 0xFFFFFF);
		_titleColor = set.getInt("titleColor", 0xECF9A2);
		_equipHead = set.getInt("equipHead", 0);
		_equipRHand = set.getInt("equipRHand", 0); // or dual hand
		_equipLHand = set.getInt("equipLHand", 0);
		_equipGloves = set.getInt("equipGloves", 0);
		_equipChest = set.getInt("equipChest", 0);
		_equipLegs = set.getInt("equipLegs", 0);
		_equipFeet = set.getInt("equipFeet", 0);
		_equipCloak = set.getInt("equipCloak", 0);
		_equipHair = set.getInt("equipHair", 0);
		_equipHair2 = set.getInt("equipHair2", 0);
		_agathionId = set.getInt("agathionId", 0);
		_weaponEnchantLevel = set.getInt("weaponEnchantLevel", 0);
		_armorEnchantLevel = set.getInt("armorEnchantLevel", 0);
		_fishing = set.getBoolean("fishing", false);
		_baitLocationX = set.getInt("baitLocationX", 0);
		_baitLocationY = set.getInt("baitLocationY", 0);
		_baitLocationZ = set.getInt("baitLocationZ", 0);
		_recommends = set.getInt("recommends", 0);
		_nobleLevel = set.getInt("nobleLevel", 0);
		_hero = set.getBoolean("hero", false);
		_clanId = set.getInt("clanId", 0);
		_pledgeStatus = set.getInt("pledgeStatus", 0);
	}

	public ClassId getClassId()
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