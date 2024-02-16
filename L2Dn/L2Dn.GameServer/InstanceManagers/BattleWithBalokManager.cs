using L2Dn.Extensions;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Serenitty, Index
 */
public class BattleWithBalokManager
{
	private readonly Map<int, int> _playerPoints = new();
	private bool _inBattle = false;
	private int _reward = 0;
	private int _globalPoints = 0;
	private int _globalStage = 0;
	private int _globalStatus = 0;
	
	public BattleWithBalokManager()
	{
	}
	
	public void addPointsForPlayer(Player player, bool isScorpion)
	{
		int pointsToAdd = isScorpion ? Config.BALOK_POINTS_PER_MONSTER * 10 : Config.BALOK_POINTS_PER_MONSTER;
		int currentPoints = _playerPoints.computeIfAbsent(player.getObjectId(), pts => 0);
		int sum = pointsToAdd + currentPoints;
		_playerPoints.put(player.getObjectId(), sum);
	}
	
	public Map<int, int> getTopPlayers(int count)
	{
		Map<int, int> result = new();
		_playerPoints.OrderByDescending(kvp => kvp.Value).Take(count).ForEach(kvp => result.put(kvp.Key, kvp.Value));
		return result;
	}
	
	public int getPlayerRank(Player player)
	{
		if (!_playerPoints.TryGetValue(player.getObjectId(), out int points))
		{
			return 0;
		}
		
		return _playerPoints.Values.Count(p => p > points) + 1; // TODO: when many players have the same amount of points 
	}
	
	public int getMonsterPoints(Player player)
	{
		return _playerPoints.computeIfAbsent(player.getObjectId(), pts => 0);
	}
	
	public int getReward()
	{
		return _reward;
	}
	
	public void setReward(int value)
	{
		_reward = value;
	}
	
	public bool getInBattle()
	{
		return _inBattle;
	}
	
	public void setInBattle(bool value)
	{
		_inBattle = value;
	}
	
	public int getGlobalPoints()
	{
		return _globalPoints;
	}
	
	public void setGlobalPoints(int value)
	{
		_globalPoints = value;
	}
	
	public int getGlobalStage()
	{
		return _globalStage;
	}
	
	public void setGlobalStage(int value)
	{
		_globalStage = value;
	}
	
	public int getGlobalStatus()
	{
		return _globalStatus;
	}
	
	public void setGlobalStatus(int value)
	{
		_globalStatus = value;
	}
	
	public static BattleWithBalokManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly BattleWithBalokManager INSTANCE = new BattleWithBalokManager();
	}
}