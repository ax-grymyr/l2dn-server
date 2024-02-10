namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Serenitty
 */
public class EnchantStarHolder
{
	private readonly int _level;
	private readonly int _expMax;
	private readonly int _expOnFail;
	private readonly long _feeAdena;

	public EnchantStarHolder(StatSet set)
	{
		_level = set.getInt("level");
		_expMax = set.getInt("expMax");
		_expOnFail = set.getInt("expOnFail");
		_feeAdena = set.getLong("feeAdena");
	}

	public int getLevel()
	{
		return _level;
	}

	public int getExpMax()
	{
		return _expMax;
	}

	public int getExpOnFail()
	{
		return _expOnFail;
	}

	public long getFeeAdena()
	{
		return _feeAdena;
	}
}