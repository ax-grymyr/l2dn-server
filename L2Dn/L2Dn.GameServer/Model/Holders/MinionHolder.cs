using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Utilities;

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
	private readonly long _respawnTime;
	private readonly int _weightPoint;
	
	public MinionHolder(StatSet set)
	{
		_id = set.getInt("id");
		_count = set.getInt("count", 1);
		_max = set.getInt("max", 0);
		_respawnTime = set.getDuration("respawnTime", Duration.ofSeconds(0)).getSeconds() * 1000;
		_weightPoint = set.getInt("weightPoint", 0);
	}
	
	/**
	 * Constructs a minion holder.
	 * @param id the id
	 * @param count the count
	 * @param max the max count
	 * @param respawnTime the respawn time
	 * @param weightPoint the weight point
	 */
	public MinionHolder(int id, int count, int max, long respawnTime, int weightPoint)
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
	public int getId()
	{
		return _id;
	}
	
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
	public long getRespawnTime()
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