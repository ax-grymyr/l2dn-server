using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Berezkin Nikolay
 */
public class PetEvolveHolder
{
	private readonly int _index;
	private readonly int _level;
	private readonly EvolveLevel _evolve;
	private readonly long _exp;
	private readonly String _name;
	
	public PetEvolveHolder(int index, EvolveLevel evolve, String name, int level, long exp)
	{
		_index = index;
		_evolve = evolve;
		_level = level;
		_exp = exp;
		_name = name;
	}
	
	public int getIndex()
	{
		return _index;
	}
	
	public EvolveLevel getEvolve()
	{
		return _evolve;
	}
	
	public int getLevel()
	{
		return _level;
	}
	
	public long getExp()
	{
		return _exp;
	}
	
	public String getName()
	{
		return _name;
	}
}