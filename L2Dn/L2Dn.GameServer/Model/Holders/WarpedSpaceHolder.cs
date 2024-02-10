using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Sdw
 */
public class WarpedSpaceHolder
{
	private readonly Creature _creature;
	private readonly int _range;

	public WarpedSpaceHolder(Creature creature, int range)
	{
		_creature = creature;
		_range = range;
	}

	public Creature getCreature()
	{
		return _creature;
	}

	public int getRange()
	{
		return _range;
	}
}