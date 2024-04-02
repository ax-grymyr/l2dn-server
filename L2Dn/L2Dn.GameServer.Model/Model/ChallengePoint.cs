using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model;

public class ChallengePoint
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ChallengePoint));
	
	private readonly Player _owner;
	private int _nowGroup;
	private int _nowPoint;
	private readonly Map<int, int> _challengePoints = new();
	private readonly Map<int, Map<int, int>> _challengePointsRecharges = new();
	private readonly int[] _challengePointsPendingRecharge =
	{
		-1,
		-1,
	};
	
	public ChallengePoint(Player owner)
	{
		_owner = owner;
		_nowGroup = 0;
		_nowPoint = 0;
	}
	
	public void storeChallengePoints()
	{
		// LOGGER.info("Storing Challenge Points for " + _owner);
		
		if (_challengePoints.isEmpty())
		{
			return;
		}

		try
		{
			// TODO: server is the owner of the database, so it should track the records to add or update
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = _owner.getObjectId();
			foreach (var pair in _challengePoints)
			{
				var record =
					ctx.EnchantChallengePoints.SingleOrDefault(r =>
						r.CharacterId == characterId && r.GroupId == pair.Key);

				if (record is null)
				{
					record = new DbEnchantChallengePoint();
					record.CharacterId = characterId;
					record.GroupId = pair.Key;
					ctx.EnchantChallengePoints.Add(record);
				}

				record.Points = pair.Value;
			}

			foreach (var pair in _challengePointsRecharges)
			{
				foreach (var pair2 in pair.Value)
				{
					var record =
						ctx.EnchantChallengePointRecharges.SingleOrDefault(r =>
							r.CharacterId == characterId && r.GroupId == pair.Key && r.OptionIndex == pair2.Key);

					if (record is null)
					{
						record = new DbEnchantChallengePointRecharge();
						record.CharacterId = characterId;
						record.GroupId = pair.Key;
						record.OptionIndex = pair2.Key;
						ctx.EnchantChallengePointRecharges.Add(record);
					}

					record.Count = pair2.Value;
				}
			}

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not store Challenge Points for " + _owner + " " + e);
		}
	}
	
	public void restoreChallengePoints()
	{
		_challengePoints.clear();
		_challengePointsRecharges.clear();

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = _owner.getObjectId();
			foreach (var record in ctx.EnchantChallengePoints.Where(r => r.CharacterId == characterId))
			{
				int groupId = record.GroupId;
				int points = record.Points;
				_challengePoints.put(groupId, points);
			}

			foreach (var record in ctx.EnchantChallengePointRecharges.Where(r => r.CharacterId == characterId))
			{
				int groupId = record.GroupId;
				int optionIndex = record.OptionIndex;
				int count = record.Count;
				Map<int, int> options = _challengePointsRecharges.get(groupId);
				if (options == null)
				{
					options = new();
					_challengePointsRecharges.put(groupId, options);
				}
				options.put(optionIndex, count);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore Challenge Points for " + _owner + " " + e);
		}
		
		// LOGGER.info("Restored Challenge Points recharges for " + _owner);
		// LOGGER.info("Restored Challenge Points for " + _owner);
	}
	
	public int getNowPoint()
	{
		int nowPoint = _nowPoint;
		_nowPoint = 0;
		return nowPoint;
	}
	
	public int getNowGroup()
	{
		int nowGroup = _nowGroup;
		_nowGroup = 0;
		return nowGroup;
	}
	
	public void setNowGroup(int val)
	{
		_nowGroup = val;
	}
	
	public void setNowPoint(int val)
	{
		_nowPoint = val;
	}
	
	public Map<int, int> getChallengePoints()
	{
		return _challengePoints;
	}
	
	public int getChallengePointsRecharges(int groupId, int optionIndex)
	{
		Map<int, int> options = _challengePointsRecharges.get(groupId);
		if (options != null)
		{
			return options.getOrDefault(optionIndex, 0);
		}
		return 0;
	}
	
	public void addChallengePointsRecharge(int groupId, int optionIndex, int amount)
	{
		Map<int, int> options = _challengePointsRecharges.get(groupId);
		if (options == null)
		{
			options = new();
			_challengePointsRecharges.put(groupId, options);
		}
		
		options.compute(optionIndex, (k, v) => v == null ? amount : v + amount);
	}
	
	public void setChallengePointsPendingRecharge(int groupId, int optionIndex)
	{
		_challengePointsPendingRecharge[0] = groupId;
		_challengePointsPendingRecharge[1] = optionIndex;
	}
	
	public int[] getChallengePointsPendingRecharge()
	{
		return _challengePointsPendingRecharge;
	}
	
	public ChallengePointInfoHolder[] initializeChallengePoints()
	{
		Map<int, int> challengePoints = getChallengePoints();
		ChallengePointInfoHolder[] info = new ChallengePointInfoHolder[challengePoints.size()];
		int i = 0;
		foreach (var entry in challengePoints)
		{
			int groupId = entry.Key;
			info[i] = new ChallengePointInfoHolder(groupId, entry.Value, //
				getChallengePointsRecharges(groupId, 0), //
				getChallengePointsRecharges(groupId, 1), //
				getChallengePointsRecharges(groupId, 2), //
				getChallengePointsRecharges(groupId, 3), //
				getChallengePointsRecharges(groupId, 4), //
				getChallengePointsRecharges(groupId, 5));
			i++;
		}
		return info;
	}
	
	public bool canAddPoints(int categoryId, int points)
	{
		int totalPoints = _challengePoints.getOrDefault(categoryId, 0) + points;
		int maxPoints = EnchantChallengePointData.getInstance().getMaxPoints();
		return maxPoints > totalPoints;
	}
}