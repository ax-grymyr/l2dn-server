using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model;

public class ChallengePoint
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ChallengePoint));
	
	// Character enchant challenge points.
	private const String INSERT_CHALLENGE_POINTS = "REPLACE INTO enchant_challenge_points (`charId`, `groupId`, `points`) VALUES (?, ?, ?)";
	private const String RESTORE_CHALLENGE_POINTS = "SELECT * FROM enchant_challenge_points WHERE charId=?";
	private const String INSERT_CHALLENGE_POINTS_RECHARGES = "REPLACE INTO enchant_challenge_points_recharges (`charId`, `groupId`, `optionIndex`, `count`) VALUES (?, ?, ?, ?)";
	private const String RESTORE_CHALLENGE_POINTS_RECHARGES = "SELECT * FROM enchant_challenge_points_recharges WHERE charId=?";
	
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
			Connection conn = DatabaseFactory.getConnection();
			PreparedStatement ps1 = conn.prepareStatement(INSERT_CHALLENGE_POINTS);
				foreach (var entry in _challengePoints.entrySet())
				{
					ps1.setInt(1, _owner.getObjectId());
					ps1.setInt(2, entry.getKey());
					ps1.setInt(3, entry.getValue());
					ps1.addBatch();
				}
				ps1.executeBatch();

				PreparedStatement ps2 = conn.prepareStatement(INSERT_CHALLENGE_POINTS_RECHARGES);
				foreach (Entry<int, Map<int, int>> entry in _challengePointsRecharges.entrySet())
				{
					foreach (Entry<int, int> entry2 in entry.getValue().entrySet())
					{
						ps2.setInt(1, _owner.getObjectId());
						ps2.setInt(2, entry.getKey());
						ps2.setInt(3, entry2.getKey());
						ps2.setInt(4, entry2.getValue());
						ps2.addBatch();
					}
				}
				ps2.executeBatch();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not store Challenge Points for " + _owner + " " + e);
		}
	}
	
	public void restoreChallengePoints()
	{
		_challengePoints.clear();
		try
		{
			using GameServerDbContext ctx = new();
			{
				PreparedStatement ps = con.prepareStatement(RESTORE_CHALLENGE_POINTS);
				ps.setInt(1, _owner.getObjectId());
				ResultSet rs = ps.executeQuery()
				while (rs.next())
				{
					final int groupId = rs.getInt("groupId");
					final int points = rs.getInt("points");
					_challengePoints.put(groupId, points);
				}
			}

			_challengePointsRecharges.clear();
			
			{
				PreparedStatement ps = con.prepareStatement(RESTORE_CHALLENGE_POINTS_RECHARGES);
				ps.setInt(1, _owner.getObjectId());
				ResultSet rs = ps.executeQuery();
					while (rs.next())
					{
						int groupId = rs.getInt("groupId");
						int optionIndex = rs.getInt("optionIndex");
						int count = rs.getInt("count");
						Map<int, int> options = _challengePointsRecharges.get(groupId);
						if (options == null)
						{
							options = new HashMap<>();
							_challengePointsRecharges.put(groupId, options);
						}
						options.put(optionIndex, count);
					}
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
		foreach (Entry<int, int> entry in challengePoints.entrySet())
		{
			int groupId = entry.getKey();
			info[i] = new ChallengePointInfoHolder(groupId, entry.getValue(), //
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