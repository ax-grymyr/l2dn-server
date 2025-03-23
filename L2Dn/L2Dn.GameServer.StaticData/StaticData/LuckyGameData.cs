using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.StaticData.Xml.LuckyGame;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class LuckyGameData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(LuckyGameData));
    private FrozenDictionary<int, LuckyGameDataHolder> _luckyGame = FrozenDictionary<int, LuckyGameDataHolder>.Empty;
    private int _serverPlay;

    private LuckyGameData()
    {
    }

    public static LuckyGameData Instance { get; } = new();

    public int ServerPlay => _serverPlay;
    public LuckyGameDataHolder? GetLuckyGameByIndex(int index) => _luckyGame.GetValueOrDefault(index);
    public int IncreaseGame() => Interlocked.Increment(ref _serverPlay);

    public void Load()
    {
        XmlLuckyGameData xmlLuckyGameData = XmlLoader.LoadXmlDocument<XmlLuckyGameData>("LuckyGameData.xml");
        _luckyGame = xmlLuckyGameData.Items.Select(Create).ToFrozenDictionary(x => x.Index);

        _logger.Info($"{nameof(LuckyGameData)}: Loaded {_luckyGame.Count} lucky game data.");
    }

    private static LuckyGameDataHolder Create(XmlLuckyGameItem item)
    {
        ImmutableArray<ItemChanceHolder> commonReward = item.CommonReward.
            Select(x => new ItemChanceHolder(x.Id, x.Count, x.Chance)).
            ToImmutableArray();

        ImmutableArray<ItemPointHolder> uniqueReward = item.UniqueReward.
            Select(x => new ItemPointHolder(x.Id, x.Count, x.Points)).
            ToImmutableArray();

        int minGame = 0;
        int maxGame = 0;
        ImmutableArray<ItemChanceHolder> modifyReward = ImmutableArray<ItemChanceHolder>.Empty;
        if (item.ModifyReward is not null)
        {
            minGame = item.ModifyReward.MinGame;
            maxGame = item.ModifyReward.MaxGame;
            modifyReward = item.ModifyReward.Items.Select(x => new ItemChanceHolder(x.Id, x.Count, x.Chance)).
                ToImmutableArray();
        }

        return new LuckyGameDataHolder(item.Index, item.TurningPoint, commonReward, uniqueReward, minGame, maxGame,
            modifyReward);
    }
}