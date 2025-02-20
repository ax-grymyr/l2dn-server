using System.Xml.Linq;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Items.Appearance;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.Utilities;

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
	
	public AppearanceHolder(XElement element)
	{
		_visualId = element.Attribute("id").GetInt32(0);
		_weaponType = element.Attribute("weaponType").GetEnum(WeaponType.NONE);
		_armorType = element.Attribute("armorType").GetEnum(ArmorType.NONE);
		_handType = element.Attribute("handType").GetEnum(AppearanceHandType.NONE);
		_magicType = element.Attribute("magicType").GetEnum(AppearanceMagicType.NONE);
		_targetType = element.Attribute("targetType").GetEnum(AppearanceTargetType.NONE);
		_bodyPart = ItemData._slotNameMap.get(element.Attribute("bodyPart").GetString("none"));
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