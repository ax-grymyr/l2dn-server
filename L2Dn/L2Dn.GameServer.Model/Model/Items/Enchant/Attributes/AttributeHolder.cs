using L2Dn.GameServer.Enums;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Items.Enchant.Attributes;

/**
 * @author UnAfraid
 */
public class AttributeHolder
{
	private readonly AttributeType _type;
	private int _value;

	public AttributeHolder(AttributeType type, int value)
	{
		_type = type;
		_value = value;
	}

	public AttributeType getType()
	{
		return _type;
	}

	public int getValue()
	{
		return _value;
	}

	public void setValue(int value)
	{
		_value = value;
	}

	public void incValue(int with)
	{
		_value += with;
	}

	public override string ToString()
	{
		return _type + " +" + _value;
	}
}