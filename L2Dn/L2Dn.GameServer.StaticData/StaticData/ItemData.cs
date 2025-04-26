using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.StaticData.Xml.Items;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/// <summary>
/// This class serves as a container for all item templates in the game.
/// </summary>
public sealed class ItemData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ItemData));

    private static readonly FrozenDictionary<string, long> _slotNameMap = new[]
    {
        ("shirt", ItemTemplate.SLOT_UNDERWEAR),
        ("lbracelet", ItemTemplate.SLOT_L_BRACELET),
        ("rbracelet", ItemTemplate.SLOT_R_BRACELET),
        ("talisman", ItemTemplate.SLOT_DECO),
        ("chest", ItemTemplate.SLOT_CHEST),
        ("fullarmor", ItemTemplate.SLOT_FULL_ARMOR),
        ("head", ItemTemplate.SLOT_HEAD),
        ("hair", ItemTemplate.SLOT_HAIR),
        ("hairall", ItemTemplate.SLOT_HAIRALL),
        ("underwear", ItemTemplate.SLOT_UNDERWEAR),
        ("back", ItemTemplate.SLOT_BACK),
        ("neck", ItemTemplate.SLOT_NECK),
        ("legs", ItemTemplate.SLOT_LEGS),
        ("feet", ItemTemplate.SLOT_FEET),
        ("gloves", ItemTemplate.SLOT_GLOVES),
        ("chest,legs", ItemTemplate.SLOT_CHEST | ItemTemplate.SLOT_LEGS),
        ("belt", ItemTemplate.SLOT_BELT),
        ("rhand", ItemTemplate.SLOT_R_HAND),
        ("lhand", ItemTemplate.SLOT_L_HAND),
        ("lrhand", ItemTemplate.SLOT_LR_HAND),
        ("rear;lear", ItemTemplate.SLOT_R_EAR | ItemTemplate.SLOT_L_EAR),
        ("rfinger;lfinger", ItemTemplate.SLOT_R_FINGER | ItemTemplate.SLOT_L_FINGER),
        ("wolf", ItemTemplate.SLOT_WOLF),
        ("greatwolf", ItemTemplate.SLOT_GREATWOLF),
        ("hatchling", ItemTemplate.SLOT_HATCHLING),
        ("strider", ItemTemplate.SLOT_STRIDER),
        ("babypet", ItemTemplate.SLOT_BABYPET),
        ("brooch", ItemTemplate.SLOT_BROOCH),
        ("brooch_jewel", ItemTemplate.SLOT_BROOCH_JEWEL),
        ("agathion", ItemTemplate.SLOT_AGATHION),
        ("artifactbook", ItemTemplate.SLOT_ARTIFACT_BOOK),
        ("artifact", ItemTemplate.SLOT_ARTIFACT),
        ("none", ItemTemplate.SLOT_NONE),

        // retail compatibility
        ("onepiece", ItemTemplate.SLOT_FULL_ARMOR),
        ("hair2", ItemTemplate.SLOT_HAIR2),
        ("dhair", ItemTemplate.SLOT_HAIRALL),
        ("alldress", ItemTemplate.SLOT_ALLDRESS),
        ("deco1", ItemTemplate.SLOT_DECO),
        ("waist", ItemTemplate.SLOT_BELT),
    }.ToFrozenDictionary(x => x.Item1, x => x.Item2, StringComparer.OrdinalIgnoreCase);

    private FrozenDictionary<int, ItemTemplate> _items = FrozenDictionary<int, ItemTemplate>.Empty;
    private FrozenDictionary<int, Armor> _armors = FrozenDictionary<int, Armor>.Empty;
    private FrozenDictionary<int, Weapon> _weapons = FrozenDictionary<int, Weapon>.Empty;
    private FrozenDictionary<int, EtcItem> _etcItems = FrozenDictionary<int, EtcItem>.Empty;
    private FrozenSet<int> _ammunitionSkillIds = FrozenSet<int>.Empty;

    private ItemData()
    {
    }

    public static FrozenDictionary<string, long> SlotNameMap => _slotNameMap;
    public FrozenSet<int> AmmunitionSkillIds => _ammunitionSkillIds;

    public void Load()
    {
        IEnumerable<XmlItemList> xmlItemLists = XmlLoader.LoadXmlDocuments<XmlItemList>("stats/items");
        if (Config.General.CUSTOM_ITEMS_LOAD)
            xmlItemLists = xmlItemLists.Concat(XmlLoader.LoadXmlDocuments<XmlItemList>("stats/items/custom"));

        _items = xmlItemLists.SelectMany(x => x.Items).Select(Create).ToFrozenDictionary(x => x.Id);
        _armors = _items.Values.OfType<Armor>().ToFrozenDictionary(x => x.Id);
        _weapons = _items.Values.OfType<Weapon>().ToFrozenDictionary(x => x.Id);
        _etcItems = _items.Values.OfType<EtcItem>().ToFrozenDictionary(x => x.Id);

        _ammunitionSkillIds = _etcItems.Values.Where(x => x.getItemType() == EtcItemType.ARROW ||
                x.getItemType() == EtcItemType.BOLT || x.getItemType() == EtcItemType.ELEMENTAL_ORB).
            SelectMany(x => x.getAllSkills().Select(s => s.getSkillId())).ToFrozenSet();

        _logger.Info($"{nameof(ItemData)}: Loaded {_etcItems.Count} etc items");
        _logger.Info($"{nameof(ItemData)}: Loaded {_armors.Count} armor items");
        _logger.Info($"{nameof(ItemData)}: Loaded {_weapons.Count} weapon items");
        _logger.Info($"{nameof(ItemData)}: Loaded {_etcItems.Count + _armors.Count + _weapons.Count} items in total.");
    }

    private static ItemTemplate Create(XmlItem xmlItem)
    {
        ItemParameterSet parameters = new ItemParameterSet();
        foreach (XmlItemParameter parameter in xmlItem.Parameters)
            parameters[parameter.Type] = parameter.Value;

        return xmlItem.Type switch
        {
            XmlItemType.Armor => new Armor(xmlItem, parameters),
            XmlItemType.Weapon => new Weapon(xmlItem, parameters),
            XmlItemType.EtcItem => new EtcItem(xmlItem, parameters),
            _ => throw new InvalidOperationException($"Invalid item type: {xmlItem.Type}"),
        };
    }

    /**
     * Returns the item corresponding to the item ID
     * @param id : int designating the item
     * @return Item
     */
    public ItemTemplate? getTemplate(int id)
    {
        if (_items.TryGetValue(id, out ItemTemplate? itemTemplate))
            return itemTemplate;

        _logger.Error($"Requested ItemTemplate ID={id} not found!");
        return null;

    }

    public ImmutableArray<int> getAllArmorsId()
    {
        return _armors.Keys;
    }

    public ImmutableArray<Armor> getAllArmors()
    {
        return _armors.Values;
    }

    public ImmutableArray<int> getAllWeaponsId()
    {
        return _weapons.Keys;
    }

    public ImmutableArray<Weapon> getAllWeapons()
    {
        return _weapons.Values;
    }

    public ImmutableArray<int> getAllEtcItemsId()
    {
        return _etcItems.Keys;
    }

    public ImmutableArray<EtcItem> getAllEtcItems()
    {
        return _etcItems.Values;
    }

    public ImmutableArray<ItemTemplate> getAllItems()
    {
        return _items.Values;
    }

    public static ItemData getInstance()
    {
        return SingletonHolder.INSTANCE;
    }

    private static class SingletonHolder
    {
        public static readonly ItemData INSTANCE = new();
    }
}