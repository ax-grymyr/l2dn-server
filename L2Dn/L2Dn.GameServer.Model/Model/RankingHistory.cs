using L2Dn.GameServer.Db;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using Microsoft.EntityFrameworkCore;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model;

public class RankingHistory
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(RankingHistory));

	private const int NUM_HISTORY_DAYS = 7;

	private readonly Player _player;
	private readonly List<RankingHistoryDataHolder> _data = new();
	private DateTime _nextUpdate;

	public RankingHistory(Player player)
	{
		_player = player;
	}

	public void store()
	{
		int ranking = RankManager.getInstance().getPlayerGlobalRank(_player);
		long exp = _player.getExp();
		DateOnly today = DateOnly.FromDateTime(DateTime.Today);
		DateOnly oldestDay = today.AddDays(-NUM_HISTORY_DAYS + 1);

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int characterId = _player.ObjectId;
			var record = ctx.CharacterRankingHistory.SingleOrDefault(r => r.CharacterId == characterId && r.Date == today);
			if (record is null)
			{
				record = new DbCharacterRankingHistory();
				record.CharacterId = characterId;
				record.Date = today;
				ctx.CharacterRankingHistory.Add(record);
			}

			record.Ranking = ranking;
			record.Exp = exp;
			ctx.SaveChanges();

			// Delete old records
			ctx.CharacterRankingHistory.Where(r => r.CharacterId == characterId && r.Date < oldestDay).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not insert RankingCharHistory data: " + e);
		}
	}

	public List<RankingHistoryDataHolder> getData()
	{
		DateTime currentTime = DateTime.UtcNow;
		if (currentTime > _nextUpdate)
		{
			_data.Clear();
			if (_nextUpdate == DateTime.MinValue) // TODO: wrong logic?
			{
				store(); // to update
			}
			_nextUpdate = currentTime.AddMilliseconds(Config.General.CHAR_DATA_STORE_INTERVAL);

			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				int characterId = _player.ObjectId;
				var query = ctx.CharacterRankingHistory.Where(r => r.CharacterId == characterId)
					.OrderByDescending(r => r.Date);

				foreach (var record in query)
				{
					DateOnly day = record.Date;
					int ranking = record.Ranking;
					long exp = record.Exp;
					_data.Add(new RankingHistoryDataHolder(day, ranking, exp));
				}
			}
			catch (Exception e)
			{
				LOGGER.Error("Could not get RankingCharHistory data: " + e);
			}
		}
		return _data;
	}
}