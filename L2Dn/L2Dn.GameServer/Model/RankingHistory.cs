using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using NLog;

namespace L2Dn.GameServer.Model;

public class RankingHistory
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(RankingHistory));
	
	private const int NUM_HISTORY_DAYS = 7;
	
	private readonly Player _player;
	private readonly List<RankingHistoryDataHolder> _data = new();
	private long _nextUpdate = 0;
	
	public RankingHistory(Player player)
	{
		_player = player;
	}
	
	public void store()
	{
		int ranking = RankManager.getInstance().getPlayerGlobalRank(_player);
		long exp = _player.getExp();
		int today = (int) (System.currentTimeMillis() / 86400000L);
		int oldestDay = (today - NUM_HISTORY_DAYS) + 1;
		
		try 
		{
			using Connection con = DatabaseFactory.getConnection();
			using PreparedStatement statement = con.prepareStatement(
				"INSERT INTO character_ranking_history (charId, day, ranking, exp) VALUES (?, ?, ?, ?) ON DUPLICATE KEY UPDATE ranking = ?, exp = ?");
			using PreparedStatement deleteSt =
				con.prepareStatement("DELETE FROM character_ranking_history WHERE charId = ? AND day < ?");
			statement.setInt(1, _player.getObjectId());
			statement.setInt(2, today);
			statement.setInt(3, ranking);
			statement.setLong(4, exp);
			statement.setInt(5, ranking); // update
			statement.setLong(6, exp); // update
			statement.execute();
			
			// Delete old records
			deleteSt.setInt(1, _player.getObjectId());
			deleteSt.setInt(2, oldestDay);
			deleteSt.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not insert RankingCharHistory data: " + e);
		}
	}
	
	public List<RankingHistoryDataHolder> getData()
	{
		long currentTime = System.currentTimeMillis();
		if (currentTime > _nextUpdate)
		{
			_data.Clear();
			if (_nextUpdate == 0)
			{
				store(); // to update
			}
			_nextUpdate = currentTime + Config.CHAR_DATA_STORE_INTERVAL;
			try 
			{
				using Connection con = DatabaseFactory.getConnection();
				using PreparedStatement statement =
					con.prepareStatement("SELECT * FROM character_ranking_history WHERE charId = ? ORDER BY day DESC");
				statement.setInt(1, _player.getObjectId());
				using ResultSet rset = statement.executeQuery();
					while (rset.next())
					{
						int day = rset.getInt("day");
						long timestamp = (day * 86400000L) + 86400000L;
						int ranking = rset.getInt("ranking");
						long exp = rset.getLong("exp");
						_data.Add(new RankingHistoryDataHolder(timestamp / 1000, ranking, exp));
					}
			}
			catch (Exception e)
			{
				LOGGER.Warn("Could not get RankingCharHistory data: " + e);
			}
		}
		return _data;
	}
}