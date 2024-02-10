using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Items.Appearance;
using L2Dn.GameServer.Model.Items.Types;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Sdw
 */
public class AppearanceHolder
{
	private readonly int _visualId;
	private readonly WeaponType _weaponType;
	private readonly ArmorType _armorType;
	private readonly AppearanceHandType _handType;
	private readonly AppearanceMagicType _magicType;
	private readonly AppearanceTargetType _targetType;
	private readonly long _bodyPart;
	
	public AppearanceHolder(StatSet set)
	{
		_visualId = set.getInt("id", 0);
		_weaponType = set.getEnum("weaponType", WeaponType.NONE);
		_armorType = set.getEnum("armorType", ArmorType.NONE);
		_handType = set.getEnum("handType", AppearanceHandType.NONE);
		_magicType = set.getEnum("magicType", AppearanceMagicType.NONE);
		_targetType = set.getEnum("targetType", AppearanceTargetType.NONE);
		_bodyPart = ItemData.SLOTS.get(set.getString("bodyPart", "none"));
	}
	
	public WeaponType getWeaponType()
	{
		return _weaponType;
	}
	
	public ArmorType getArmorType()
	{
		return _armorType;
	}
	
	public AppearanceHandType getHandType()
	{
		return _handType;
	}
	
	public AppearanceMagicType getMagicType()
	{
		return _magicType;
	}
	
	public AppearanceTargetType getTargetType()
	{
		return _targetType;
	}
	
	public long getBodyPart()
	{
		return _bodyPart;
	}
	
	public int getVisualId()
	{
		return _visualId;
	}
}