using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * This class describes a RecipeList statUse and altStatChange component.
 */
public class RecipeStatHolder
{
	/** The Identifier of the statType */
	private readonly StatType _type;

	/** The value of the statType */
	private readonly int _value;

	/**
	 * Constructor of RecipeStatHolder.
	 * @param type
	 * @param value
	 */
	public RecipeStatHolder(String type, int value)
	{
		_type = Enum.Parse<StatType>(type); // TODO: pass value, not string
		_value = value;
	}

	/**
	 * @return the the type of the RecipeStatHolder.
	 */
	public StatType getType()
	{
		return _type;
	}

	/**
	 * @return the value of the RecipeStatHolder.
	 */
	public int getValue()
	{
		return _value;
	}
}