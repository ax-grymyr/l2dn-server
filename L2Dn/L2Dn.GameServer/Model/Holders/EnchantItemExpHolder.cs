namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Serenitty
 */
public class EnchantItemExpHolder
{
	private readonly int _id;
	private readonly int _exp;
	private readonly int _starLevel;

	public EnchantItemExpHolder(int id, int exp, int starLevel)
	{
		_id = id;
		_exp = exp;
		_starLevel = starLevel;
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