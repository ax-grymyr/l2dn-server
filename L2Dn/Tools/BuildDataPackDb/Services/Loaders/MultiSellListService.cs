using System.Globalization;
using L2Dn.DataPack.Db;
using L2Dn.Model.Xml;
using L2Dn.Utilities;
using NLog;

namespace BuildDataPackDb.Services.Loaders;

public class MultiSellListService
{
    private readonly Logger _logger = LogManager.GetLogger(nameof(BuyListService));
    private readonly string _dataPackPath;
    private readonly DatabaseService _databaseService;

    public MultiSellListService(string dataPackPath, DatabaseService databaseService)
    {
        _dataPackPath = dataPackPath;
        _databaseService = databaseService;
    }

    public void Load()
    {
        string multiSellListPath = Path.Combine(_dataPackPath, "multisell");
        _logger.Info($"Loading multisell lists from '{multiSellListPath}'...");

        using DataPackDbContext ctx = _databaseService.CreateContext();

        IEnumerable<string> files = Directory.EnumerateFiles(multiSellListPath, "*.xml", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            XmlMultiSellList list = XmlUtil.Deserialize<XmlMultiSellList>(file);
            int listId = int.Parse(Path.GetFileNameWithoutExtension(file), CultureInfo.InvariantCulture);
            ctx.MultiSellLists.Add(new DbMultiSellList()
            {
                MultiSellListId = listId,
                Name = $"Multisell list {listId}",
                ApplyTaxes = list.ApplyTaxes,
                IsChanceMultiSell = list.IsChanceMultiSell,
                MaintainEnchantment = list.MaintainEnchantment,
                IngredientMultiplier = list.IngredientMultiplier,
                ProductMultiplier = list.ProductMultiplier,
            });
            
            ctx.MultiSellListNpcs.AddRange(list.Npcs.Select(x => new DbMultiSellListNpc()
            {
                MultiSellListId = listId,
                NpcId = x
            }));

            List<DbMultiSellListEntry> entries = new List<DbMultiSellListEntry>();
            for (int index = 0; index < list.Items.Count; index++)
            {
                DbMultiSellListEntry dbEntry = new DbMultiSellListEntry()
                {
                    MultiSellListId = listId,
                    Order = index,
                };
                
                entries.Add(dbEntry);
                ctx.MultiSellListEntries.Add(dbEntry);
            }

            ctx.SaveChanges();
            
            for (int index = 0; index < list.Items.Count; index++)
            {
                XmlMultiSellListItem entry = list.Items[index];
                DbMultiSellListEntry dbEntry = entries[index];

                ctx.MultiSellListIngredients.AddRange(entry.Ingredients.Select((x, i) => new DbMultiSellListIngredient()
                {
                    MultiSellListEntryId = dbEntry.MultiSellListEntryId,
                    Order = i,
                    ItemId = x.ItemId,
                    Count = x.Count,
                    EnchantLevel = x.EnchantLevel,
                    MaintainIngredient = x.MaintainIngredient
                }));

                ctx.MultiSellListProducts.AddRange(entry.Products.Select((x, i) => new DbMultiSellListProduct()
                {
                    MultiSellListEntryId = dbEntry.MultiSellListEntryId,
                    Order = i,
                    ItemId = x.ItemId,
                    Count = x.Count,
                    EnchantLevel = x.EnchantLevel,
                    Chance = x.Chance,
                }));
            }
        }

        ctx.SaveChanges();
    }
}