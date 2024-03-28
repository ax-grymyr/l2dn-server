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

	public EnchantStarHolder(int level, int expMax, int expOnFail, long feeAdena)
	{
		_level = level;
		_expMax = expMax;
		_expOnFail = expOnFail;
		_feeAdena = feeAdena;
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