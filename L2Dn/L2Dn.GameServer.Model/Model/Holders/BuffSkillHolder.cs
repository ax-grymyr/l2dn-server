namespace L2Dn.GameServer.Model.Holders;

/**
 * A container used for schemes buffer.
 */
public class BuffSkillHolder
{
	private readonly int _id;
	private readonly int _level;
	private readonly int _price;
	private readonly string _type;
	private readonly string _description;
	
	public BuffSkillHolder(int id, int level, int price, string type, string description)
	{
		_id = id;
		_level = level;
		_price = price;
		_type = type;
		_description = description;
	}
	
	public int getId()
	{
		return _id;
	}
	
	public int getLevel()
	{
		return _level;
	}
	
	public int getPrice()
	{
		return _price;
	}
	
	public string getType()
	{
		return _type;
	}
	
	public string getDescription()
	{
		return _description;
	}
}