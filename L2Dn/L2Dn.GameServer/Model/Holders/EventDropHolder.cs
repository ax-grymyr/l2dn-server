using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class EventDropHolder: DropHolder
{
	private readonly int _minLevel;
	private readonly int _maxLevel;
	private readonly Set<int> _monsterIds;

	public EventDropHolder(int itemId, long min, long max, double chance, int minLevel, int maxLevel,
		Set<int> monsterIds): base(null, itemId, min, max, chance)
	{
		_minLevel = minLevel;
		_maxLevel = maxLevel;
		_monsterIds = monsterIds;
	}

	public int getMinLevel()
	{
		return _minLevel;
	}

	public int getMaxLevel()
	{
		return _maxLevel;
	}

	public Set<int> getMonsterIds()
	{
		return _monsterIds;
	}
}