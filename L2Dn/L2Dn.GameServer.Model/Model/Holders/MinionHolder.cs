using L2Dn.GameServer.Dto;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Holders;

/**
 * This class hold info needed for minions spawns<br>
 * @author Zealar
 */
public class MinionHolder: IIdentifiable
{
	private readonly int _id;
	private readonly int _count;
	private readonly int _max;
	private readonly TimeSpan _respawnTime;
	private readonly int _weightPoint;

	/**
	 * Constructs a minion holder.
	 * @param id the id
	 * @param count the count
	 * @param max the max count
	 * @param respawnTime the respawn time
	 * @param weightPoint the weight point
	 */
	public MinionHolder(int id, int count, int max, TimeSpan respawnTime, int weightPoint)
	{
		_id = id;
		_count = count;
		_max = max;
		_respawnTime = respawnTime;
		_weightPoint = weightPoint;
	}

    /**
	 * @return the Identifier of the Minion to spawn.
	 */
    public int Id => _id;

    /**
	 * @return the count of the Minions to spawn.
	 */
	public int getCount()
	{
		if (_max > _count)
		{
			return Rnd.get(_count, _max);
		}
		return _count;
	}

	/**
	 * @return the respawn time of the Minions.
	 */
	public TimeSpan getRespawnTime()
	{
		return _respawnTime;
	}

	/**
	 * @return the weight point of the Minion.
	 */
	public int getWeightPoint()
	{
		return _weightPoint;
	}
}