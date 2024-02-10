namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Serenitty
 */
public class EnchantItemExpHolder
{
	private readonly int _id;
	private readonly int _exp;
	private readonly int _starLevel;

	public EnchantItemExpHolder(StatSet set)
	{
		_id = set.getInt("id", 1);
		_exp = set.getInt("exp", 1);
		_starLevel = set.getInt("starLevel", 1);
	}

	public int getStarLevel()
	{
		return _starLevel;
	}

	public int getId()
	{
		return _id;
	}

	public int getExp()
	{
		return _exp;
	}
}