using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author L2CCCP
 */
public class MagicLampHolder
{
	private readonly MagicLampDataHolder _lamp;
	private int _count;
	private long _exp;
	private long _sp;
	
	public MagicLampHolder(MagicLampDataHolder lamp)
	{
		_lamp = lamp;
	}
	
	public void inc()
	{
		_count++;
		_exp += _lamp.getExp();
		_sp += _lamp.getSp();
	}
	
	public LampType getType()
	{
		return _lamp.getType();
	}
	
	public int getCount()
	{
		return _count;
	}
	
	public long getExp()
	{
		return _exp;
	}
	
	public long getSp()
	{
		return _sp;
	}
}