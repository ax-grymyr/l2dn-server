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
		return _playerPoints.entrySet().stream().sorted(Entry.comparingByValue(Comparator.reverseOrder())).limit(count).collect(Collectors.toMap(Entry::getKey, Entry::getValue, (e1, e2) => e1, LinkedHashMap::new));
	}
	
	public int getPlayerRank(Player player)
	{
		if (!_playerPoints.containsKey(player.getObjectId()))
		{
			return 0;
		}
		
		Map<int, int> sorted = _playerPoints.entrySet().stream().sorted(Entry.comparingByValue(Comparator.reverseOrder())).collect(Collectors.toMap(Entry::getKey, Entry::getValue, (e1, e2) => e1, LinkedHashMap::new));
		return sorted.Keys.stream().toList().indexOf(player.getObjectId()) + 1;
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