using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.Model.DailyMissions;

public class PlayerDailyMissionList
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(PlayerDailyMissionList));
    private readonly Player _owner;
    private readonly Map<int, DailyMissionPlayerEntry> _entries = new();
    private bool _restored;

    public PlayerDailyMissionList(Player player)
    {
        _owner = player;
    }

    public int getAvailableCount()
    {
        if (!_restored)
            restore();

        return _entries.Count(r => r.Value.getStatus() == DailyMissionStatus.AVAILABLE);
    }

    private void restore()
    {
        int characterId = _owner.ObjectId;

        try
        {
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            var query = ctx.CharacterDailyRewards.Where(r => r.CharacterId == characterId);
            foreach (var record in query)
            {
                DailyMissionPlayerEntry entry = new DailyMissionPlayerEntry(record.CharacterId, record.RewardId,
                    record.Status, record.Progress, record.LastCompleted);

                _entries[record.RewardId] = entry;
            }

            _restored = true;
        }
        catch (Exception e)
        {
            _logger.Warn("Error while loading rewards for player: " + characterId + " from database: " + e);
        }
    }

    public void storeEntry(DailyMissionPlayerEntry entry)
    {
        int characterId = _owner.ObjectId;
        int rewardId = entry.getRewardId();

        try
        {
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            ctx.CharacterDailyRewards.Where(r => r.CharacterId == characterId && r.RewardId == rewardId)
                .ExecuteDelete();

            ctx.CharacterDailyRewards.Add(new DbCharacterDailyReward()
            {
                CharacterId = characterId,
                RewardId = rewardId,
                Status = entry.getStatus(),
                Progress = entry.getProgress(),
                LastCompleted = entry.getLastCompleted()
            });

            ctx.SaveChanges();
        }
        catch (Exception e)
        {
            _logger.Warn("Error while storing rewards for player: " + characterId + " to database: " + e);
        }
    }

    public void store()
    {
        int characterId = _owner.ObjectId;

        try
        {
            // TODO: the server is the owner of all database data, it can track which records need to be stored
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            ctx.CharacterDailyRewards.Where(r => r.CharacterId == characterId).ExecuteDelete();

            ctx.CharacterDailyRewards.AddRange(_entries.Values.Select(e => new DbCharacterDailyReward()
            {
                CharacterId = characterId,
                RewardId = e.getRewardId(),
                Status = e.getStatus(),
                Progress = e.getProgress(),
                LastCompleted = e.getLastCompleted()
            }));

            ctx.SaveChanges();
        }
        catch (Exception e)
        {
            _logger.Warn("Error while storing rewards for player: " + characterId + " to database: " + e);
        }
    }

    public DailyMissionStatus getStatus(int rewardId)
    {
        if (!_restored)
            restore();
        
        if (_entries.TryGetValue(rewardId, out DailyMissionPlayerEntry? entry))
            return entry.getStatus();

        return DailyMissionStatus.NOT_AVAILABLE;
    }

    public int getProgress(int rewardId)
    {
        if (!_restored)
            restore();
        
        return _entries.TryGetValue(rewardId, out DailyMissionPlayerEntry? entry) ? entry.getProgress() : 0;
    }

    public bool isRecentlyCompleted(int rewardId)
    {
        if (!_restored)
            restore();

        return _entries.TryGetValue(rewardId, out DailyMissionPlayerEntry? entry) && entry.isRecentlyCompleted();
    }

    public DailyMissionPlayerEntry getOrCreateEntry(int rewardId)
    {
        if (!_restored)
            restore();

        return _entries.GetOrAdd(rewardId,
            static (key, self) => new DailyMissionPlayerEntry(self._owner.getId(), key),
            this);
    }

    public DailyMissionPlayerEntry? getEntry(int rewardId)
    {
        if (!_restored)
            restore();
        
        return _entries.GetValueOrDefault(rewardId);
    }

    public void reset(int rewardId, bool deleteFromDb)
    {
        if (_entries.TryRemove(rewardId, out _) && deleteFromDb)
        {
            int characterId = _owner.ObjectId;

            try
            {
                using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
                ctx.CharacterDailyRewards.Where(r => r.CharacterId == characterId && r.RewardId == rewardId)
                    .ExecuteDelete();
            }
            catch (Exception e)
            {
                _logger.Warn("Error while deleting reward for player: " + characterId + " from database: " + e);
            }
        }
    }
}