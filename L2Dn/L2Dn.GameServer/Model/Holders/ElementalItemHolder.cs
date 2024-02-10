using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class ElementalItemHolder
{
	private readonly int _itemId;
	private readonly AttributeType _element;
	private readonly ElementalItemType _type;
	private readonly int _power;
	
	public ElementalItemHolder(int itemId, AttributeType element, ElementalItemType type, int power)
	{
		_itemId = itemId;
		_element = element;
		_type = type;
		_power = power;
	}
	
	public int getItemId()
	{
		return _itemId;
	}
	
	public AttributeType getElement()
	{
		return _element;
	}
	
	public ElementalItemType getType()
	{
		return _type;
	}
	
	public int getPower()
	{
		return _power;
	}
}