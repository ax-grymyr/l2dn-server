using System.Globalization;
using System.Xml.Serialization;
using BuildDataPackDb.Db;
using NLog;

namespace BuildDataPackDb.Services.Loaders;

public class BuyListService
{
    private readonly Logger _logger = LogManager.GetLogger(nameof(BuyListService));
    private readonly string _dataPackPath;
    private readonly DatabaseService _databaseService;

    public BuyListService(string dataPackPath, DatabaseService databaseService)
    {
        _dataPackPath = dataPackPath;
        _databaseService = databaseService;
    }

    public void Load()
    {
        string buyListPath = Path.Combine(_dataPackPath, "buylists");
        _logger.Info($"Loading buy lists from '{buyListPath}'...");

        XmlSerializer serializer = new(typeof(XmlBuyList));
        using DataPackDbContext ctx = _databaseService.CreateContext();

        IEnumerable<string> files = Directory.EnumerateFiles(buyListPath, "*.xml", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            using FileStream reader = new(file, FileMode.Open, FileAccess.Read, FileShare.None);
            XmlBuyList? list = (XmlBuyList?)serializer.Deserialize(reader);
            if (list == null)
            {
                _logger.Warn($"Cannot load buy list '{file}'");
                continue;
            }
            
            int listId = int.Parse(Path.GetFileNameWithoutExtension(file), CultureInfo.InvariantCulture);
            ctx.BuyLists.Add(new DbBuyList() { BuyListId = listId });
            ctx.BuyListNpcs.AddRange(list.NpcIds.Select(x => new DbBuyListNpc()
            {
                BuyListId = listId,
                NpcId = x
            }));

            ctx.BuyListItems.AddRange(list.Items.Select(x => new DbBuyListItem()
            {
                BuyListId = listId,
                ItemId = x.Id,
                Price = x.Price,
                Count = x.CountSpecified ? x.Count : null,
                RestockDelay = x.RestockDelaySpecified ? x.RestockDelay : null,
            }));
        }

        ctx.SaveChanges();
    }
}

[XmlRoot("list")]
public class XmlBuyList
{
    [XmlArray(ElementName = "npcs")]
    [XmlArrayItem(ElementName = "npc")]
    public List<int> NpcIds { get; set; } = [];

    [XmlElement("item")]
    public List<XmlBuyListItem> Items { get; set; } = [];
}

public class XmlBuyListItem
{
    [XmlAttribute(AttributeName = "id")]
    public int Id { get; set; }

    [XmlAttribute(AttributeName = "price")]
    public long Price { get; set; }

    [XmlAttribute(AttributeName = "count")]
    public int Count { get; set; }
    
    [XmlIgnore]
    public bool CountSpecified { get; set; }

    [XmlAttribute(AttributeName = "restock_delay")]
    public int RestockDelay { get; set; }
    
    [XmlIgnore]
    public bool RestockDelaySpecified { get; set; }
}