using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author L2CCCP, Serenitty
 */
public class MagicLampDataHolder
{
	private readonly LampType _type;
	private readonly long _exp;
	private readonly long _sp;
	private readonly double _chance;
	private readonly int _fromLevel;
	private readonly int _toLevel;

	public MagicLampDataHolder(LampType type, long exp, long sp, double chance, int fromLevel, int toLevel)
	{
		_type = type;
		_exp = exp;
		_sp = sp;
		_chance = chance;
		_fromLevel = fromLevel;
		_toLevel = toLevel;
	}

	public LampType getType()
	{
		return _type;
	}

	public long getExp()
	{
		return _exp;
	}

	public long getSp()
	{
		return _sp;
	}

	public double getChance()
	{
		return _chance;
	}

	public int getFromLevel()
	{
		return _fromLevel;
	}

	public int getToLevel()
	{
		return _toLevel;
	}
}