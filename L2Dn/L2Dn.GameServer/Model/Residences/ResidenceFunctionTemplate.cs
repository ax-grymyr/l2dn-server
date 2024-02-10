using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Model.Residences;

/**
 * @author UnAfraid
 */
public class ResidenceFunctionTemplate
{
	private readonly int _id;
	private readonly int _level;
	private readonly ResidenceFunctionType _type;
	private readonly ItemHolder _cost;
	private readonly TimeSpan _duration;
	private readonly double _value;
	
	public ResidenceFunctionTemplate(StatSet set)
	{
		_id = set.getInt("id");
		_level = set.getInt("level");
		_type = set.getEnum("type", ResidenceFunctionType.NONE);
		_cost = new ItemHolder(set.getInt("costId"), set.getLong("costCount"));
		_duration = set.getDuration("duration");
		_value = set.getDouble("value", 0);
	}
	
	/**
	 * @return the function id
	 */
	public int getId()
	{
		return _id;
	}
	
	/**
	 * @return the function level
	 */
	public int getLevel()
	{
		return _level;
	}
	
	/**
	 * @return the function type
	 */
	public ResidenceFunctionType getType()
	{
		return _type;
	}
	
	/**
	 * @return the cost of the function
	 */
	public ItemHolder getCost()
	{
		return _cost;
	}
	
	/**
	 * @return the duration of the function
	 */
	public TimeSpan getDuration()
	{
		return _duration;
	}
	
	/**
	 * @return the duration of the function as days
	 */
	public long getDurationAsDays()
	{
		return (long)_duration.TotalDays;
	}
	
	/**
	 * @return value of the function
	 */
	public double getValue()
	{
		return _value;
	}
}
