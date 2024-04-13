using L2Dn.DataPack.Db;
using L2Dn.IO;
using L2Dn.Packages.DatDefinitions;
using L2Dn.Packages.DatDefinitions.Definitions;
using L2Dn.Packages.DatDefinitions.Definitions.Enums;
using NLog;
using ItemDefaultAction = L2Dn.DataPack.Db.ItemDefaultAction;

namespace BuildDataPackDb.Services;

public class ClientDataService
{
    private readonly Logger _logger = LogManager.GetLogger(nameof(ClientDataService));
    private readonly FileLocationService _fileLocationService;
    private readonly DatabaseService _databaseService;
    private readonly IconService _iconService;

    public ClientDataService(FileLocationService fileLocationService, DatabaseService databaseService,
        IconService iconService)
    {
        _fileLocationService = fileLocationService;
        _databaseService = databaseService;
        _iconService = iconService;
    }

    public void Load()
    {
        _iconService.LoadIconPackage();

        EncryptionKeys.RsaDecryption413 = EncryptionKeys.RsaDecryption413L2EncDec;
        LoadingNameData();
        LoadingItemNames();
        LoadingArmorData();
        LoadingWeaponData();
        LoadingEtcItemData();
    }

    private void LoadingNameData()
    {
        string l2NameDataPath = _fileLocationService.L2GameDataPath;
        _logger.Info($"Loading name data from '{l2NameDataPath}'...");
        
        DatReader.ReadNameData(l2NameDataPath);
    }
    
    private void LoadingItemNames()
    {
        string itemNamePath = _fileLocationService.GetClientFilePath("ItemName_Classic-*.dat", _fileLocationService.ClientDatPath);
        _logger.Info($"Loading items from '{itemNamePath}'...");
        
        ItemNameV18 itemName = DatReader.Read<ItemNameV18>(itemNamePath);

        using DataPackDbContext ctx = _databaseService.CreateContext();
        ctx.Items.AddRange(itemName.Records.Select(x => new DbItem()
        {
            ItemId = (int)x.Id,
            Name = x.Name.Text,
            AdditionalName = x.AdditionalName,
            Description = x.Description,

            KeepType = x.KeepType,
            NameClass = x.NameClass,
            Color = x.Color,
            Popup = x.Popup,
            EnchantBless = (int)x.EnchantBless,

            DefaultAction = (ItemDefaultAction)x.DefaultAction,

            UseOrder = (int)x.UseOrder,
            SortOrder = x.SortOrder,
            AuctionCategory = (int)x.AuctionCategory,

            Flags = (x.IsTrade != 0 ? ItemFlags.Trade : ItemFlags.None) |
                    (x.IsDrop != 0 ? ItemFlags.Drop : ItemFlags.None) |
                    (x.IsDestruct != 0 ? ItemFlags.Destruct : ItemFlags.None) |
                    (x.IsPrivateStore != 0 ? ItemFlags.PrivateStore : ItemFlags.None) |
                    (x.IsNpcTrade != 0 ? ItemFlags.NpcTrade : ItemFlags.None) |
                    (x.IsCommissionStore != 0 ? ItemFlags.CommissionStore : ItemFlags.None)
        }));

        ctx.SaveChanges();

        List<(ItemNameV18.CreateItem, DbItemCreateList)> createLists = new(); 
        foreach (var record in itemName.Records)
        {
            for (int index = 0; index < record.CreateItemList.Length; index++)
            {
                ItemNameV18.CreateItem list = record.CreateItemList[index];
                var createList = new DbItemCreateList()
                {
                    BoxItemId = (int)record.Id,
                    Index = index,
                    Type = (ItemCreateListType)list.DropType,
                };

                ctx.ItemCreateLists.Add(createList);
                createLists.Add((list, createList));
            }
        }

        ctx.SaveChanges();

        foreach (var (createList, dbCreateList) in createLists)
        {
            ctx.ItemCreateListItems.AddRange(createList.RandomCreateItemList.Select((x, idx) =>
                new DbItemCreateListItem()
                {
                    ListId = dbCreateList.ListId,
                    Index = idx,
                    ItemId = (int)x.ItemClassId,
                    Count = x.Count,
                    EnchantValue = (int)x.EnchantValue
                }));
        }

        ctx.SaveChanges();
        _logger.Info($"{itemName.Records.Length} items saved");
    }

    private void LoadingArmorData()
    {
        string itemNamePath = _fileLocationService.GetClientFilePath("Armorgrp_Classic.dat", _fileLocationService.ClientDatPath);
        _logger.Info($"Loading armor data from '{itemNamePath}'...");
        
        ArmorGrpV14 armorGrp = DatReader.Read<ArmorGrpV14>(itemNamePath);

        using DataPackDbContext ctx = _databaseService.CreateContext();
            
        // Preload items
        Dictionary<int, DbItem> items = ctx.Items.ToDictionary(x => x.ItemId);
            
        int counter = 0;
        foreach (var record in armorGrp.Records)
        {
            int itemId = (int)record.ObjectId;
            if (!items.TryGetValue(itemId, out DbItem? item))
            {
                _logger.Warn($"No item with id={itemId}");
                continue;
            }

            counter++;

            item.Type = ItemType.Armor;
            
            if (record.Crystallizable != 0)
                item.Flags |= ItemFlags.Crystallizable;

            if (record.IsAttribution != 0)
                item.Flags |= ItemFlags.Attribution;

            if (record.Durability != -1)
                item.Durability = record.Durability;

            item.Weight = record.Weight;
            item.InventoryType = (ItemInventoryType)record.InventoryType;
            item.BodyPart = (ItemBodyPart)record.BodyPart;
            item.MaterialType = (ItemMaterialType)record.MaterialType;
            item.ArmorType = (ItemArmorType)record.ArmorType;
            item.Grade = (ItemGrade)record.CrystalType;
            item.MpBonus = record.MpBonus;
            item.FullArmorEnchantEffectType = record.FullArmorEnchantEffectType;
            item.NormalEnsoulCount = record.NormalEnsoulCount;
            item.SpecialEnsoulCount = record.SpecialEnsoulCount;

            int iconCount = record.Icons.Count(x => x.Text != "None");
            IconType iconType1;
            IconType iconType2 = IconType.None;
            IconType iconType3 = IconType.None;
            IconType iconType4 = IconType.None;
            IconType iconType5 = IconType.None;
            if (record.BodyPart == BodyPartV2Type.OnePiece)
            {
                if (iconCount != 3)
                    _logger.Trace($"Armor {itemId} has {iconCount} icons for body part {record.BodyPart}");

                iconType1 = IconType.OnePiece;
                iconType2 = IconType.UpperBody;
                iconType3 = IconType.LowerBody;
            }
            else if (record.BodyPart == BodyPartV2Type.AllDress)
            {
                if (iconCount != 5)
                    _logger.Trace($"Armor {itemId} has {iconCount} icons for body part {record.BodyPart}");
                
                iconType1 = IconType.OnePiece;
                iconType2 = IconType.UpperBody;
                iconType3 = IconType.LowerBody;
                iconType4 = IconType.Gloves;
                iconType5 = IconType.Boots;
            }
            else
            {
                iconType1 = GetIconTypeFromBodyPart((ItemBodyPart)record.BodyPart);
                if (iconCount != 1)
                    _logger.Trace($"Armor {itemId} has {iconCount} icons for body part {record.BodyPart}");
            }
            
            // icons
            item.Icon1Id = _iconService.GetIconId(record.Icons[0].Text, iconType1, itemId);
            item.Icon2Id = _iconService.GetIconId(record.Icons[1].Text, iconType2, itemId);
            item.Icon3Id = _iconService.GetIconId(record.Icons[2].Text, iconType3, itemId);
            item.Icon4Id = _iconService.GetIconId(record.Icons[3].Text, iconType4, itemId);
            item.Icon5Id = _iconService.GetIconId(record.Icons[4].Text, iconType5, itemId);
            item.PanelIconId = _iconService.GetIconId(record.IconPanel.Text, IconType.IconPanel, itemId);
            
            // quests
            ctx.ItemRelatedQuests.AddRange(record.RelatedQuests.Select(x => new DbItemRelatedQuest()
            {
                ItemId = itemId,
                QuestId = x.QuestId
            }));
        }

        ctx.SaveChanges();
        _logger.Info($"{counter} armor items saved");
    }

    private void LoadingWeaponData()
    {
        string itemNamePath = _fileLocationService.GetClientFilePath("Weapongrp_Classic.dat", _fileLocationService.ClientDatPath);
        _logger.Info($"Loading weapon data from '{itemNamePath}'...");
        
        WeaponGrpV12 weaponGrp = DatReader.Read<WeaponGrpV12>(itemNamePath);

        using DataPackDbContext ctx = _databaseService.CreateContext();
            
        // Preload items
        Dictionary<int, DbItem> items = ctx.Items.ToDictionary(x => x.ItemId);
            
        int counter = 0;
        foreach (var record in weaponGrp.Records)
        {
            int itemId = (int)record.ObjectId;
            if (!items.TryGetValue(itemId, out DbItem? item))
            {
                _logger.Warn($"No item with id={itemId}");
                continue;
            }

            if (item.Type != ItemType.Unknown)
            {
                _logger.Warn($"Item {itemId} type conflict: {item.Type} in db, but item in weapon data");
                continue;
            }
            
            counter++;

            item.Type = ItemType.Weapon;
            
            if (record.Crystallizable != 0)
                item.Flags |= ItemFlags.Crystallizable;

            if (record.IsAttribution != 0)
                item.Flags |= ItemFlags.Attribution;

            if (record.IsMagicWeapon != 0)
                item.Flags |= ItemFlags.MagicWeapon;

            switch (record.CanEquipHero)
            {
                case 1:
                    item.Flags |= ItemFlags.HeroWeapon;
                    break;
                
                case 255: // usual weapon
                    break;

                default:
                    throw new NotImplementedException();
            }
            
            if (record.Durability != -1)
                item.Durability = record.Durability;

            item.Weight = record.Weight;
            item.InventoryType = (ItemInventoryType)record.InventoryType;
            item.BodyPart = (ItemBodyPart)record.BodyPart;
            item.MaterialType = (ItemMaterialType)record.MaterialType;
            item.WeaponType = (ItemWeaponType)record.WeaponType;
            item.Grade = (ItemGrade)record.CrystalType;
            item.MpConsume = record.MpConsume;
            item.Handness = record.Handness;
            item.SoulshotCount = record.SoulshotCount;
            item.SpiritshotCount = record.SpiritshotCount;
            item.RandomDamage = record.RandomDamage;
            item.NormalEnsoulCount = record.NormalEnsoulCount;
            item.SpecialEnsoulCount = record.SpecialEnsoulCount;

            int iconCount = record.Icons.Count(x => x.Text != "None");
            IconType iconType1;
            IconType iconType2 = IconType.None;
            IconType iconType3 = IconType.None;
            IconType iconType4 = IconType.None;
            IconType iconType5 = IconType.None;

            if (record.BodyPart == BodyPartV2Type.LrHand)
            {
                iconType1 = IconType.Weapon;
                iconType2 = IconType.Weapon;
                iconType3 = IconType.Weapon;
                if (iconCount != 3 && iconCount != 1)
                    _logger.Trace($"Weapon {itemId} has {iconCount} icons for body part {record.BodyPart}");
            }
            else
            {
                iconType1 = GetIconTypeFromBodyPart((ItemBodyPart)record.BodyPart);
                if (iconCount != 1)
                    _logger.Trace($"Weapon {itemId} has {iconCount} icons for body part {record.BodyPart}");
            }

            // icons
            item.Icon1Id = _iconService.GetIconId(record.Icons[0].Text, iconType1, itemId);
            item.Icon2Id = _iconService.GetIconId(record.Icons[1].Text, iconType2, itemId);
            item.Icon3Id = _iconService.GetIconId(record.Icons[2].Text, iconType3, itemId);
            item.Icon4Id = _iconService.GetIconId(record.Icons[3].Text, iconType4, itemId);
            item.Icon5Id = _iconService.GetIconId(record.Icons[4].Text, iconType5, itemId);
            item.PanelIconId = _iconService.GetIconId(record.IconPanel.Text, IconType.IconPanel, itemId);
            
            // quests
            ctx.ItemRelatedQuests.AddRange(record.RelatedQuests.Select(x => new DbItemRelatedQuest()
            {
                ItemId = itemId,
                QuestId = x.QuestId
            }));
        }

        ctx.SaveChanges();
        _logger.Info($"{counter} weapon items saved");
    }

    private void LoadingEtcItemData()
    {
        string itemNamePath = _fileLocationService.GetClientFilePath("EtcItemgrp_Classic.dat", _fileLocationService.ClientDatPath);
        _logger.Info($"Loading etc item data from '{itemNamePath}'...");
        
        EtcItemGrpV9 etcItemGrp = DatReader.Read<EtcItemGrpV9>(itemNamePath);

        using DataPackDbContext ctx = _databaseService.CreateContext();
            
        // Preload items
        Dictionary<int, DbItem> items = ctx.Items.ToDictionary(x => x.ItemId);
            
        int counter = 0;
        foreach (var record in etcItemGrp.Records)
        {
            int itemId = (int)record.ObjectId;
            if (!items.TryGetValue(itemId, out DbItem? item))
            {
                _logger.Warn($"No item with id={itemId}");
                continue;
            }

            if (item.Type != ItemType.Unknown)
            {
                _logger.Warn($"Item {itemId} type conflict: {item.Type} in db, but item in etc data");
                continue;
            }

            counter++;

            item.Type = ItemType.Etc;
            
            if (record.Crystallizable != 0)
                item.Flags |= ItemFlags.Crystallizable;

            if (record.IsAttribution != 0)
                item.Flags |= ItemFlags.Attribution;

            if (record.Durability != -1)
                item.Durability = record.Durability;

            item.Weight = record.Weight;
            item.InventoryType = (ItemInventoryType)record.InventoryType;
            item.MaterialType = (ItemMaterialType)record.MaterialType;
            item.Grade = (ItemGrade)record.CrystalType;
            item.ConsumeType = (ItemConsumeType)record.ConsumeType;
            item.EtcItemType = (ItemEtcType)record.EtcItemType;
            item.ScrollSetId = record.ScrollSetId;

            //int iconCount = record.Icons.Count(x => x.Text != "None");
            IconType iconType1 = IconType.Etc;
            IconType iconType2 = IconType.Etc;
            IconType iconType3 = IconType.Etc;
            IconType iconType4 = IconType.Etc;
            IconType iconType5 = IconType.Etc;
            
            // icons
            item.Icon1Id = _iconService.GetIconId(record.Icons[0].Text, iconType1, itemId);
            item.Icon2Id = _iconService.GetIconId(record.Icons[1].Text, iconType2, itemId);
            item.Icon3Id = _iconService.GetIconId(record.Icons[2].Text, iconType3, itemId);
            item.Icon4Id = _iconService.GetIconId(record.Icons[3].Text, iconType4, itemId);
            item.Icon5Id = _iconService.GetIconId(record.Icons[4].Text, iconType5, itemId);
            item.PanelIconId = _iconService.GetIconId(record.IconPanel.Text, IconType.IconPanel, itemId);
            
            // quests
            ctx.ItemRelatedQuests.AddRange(record.RelatedQuests.Select(x => new DbItemRelatedQuest()
            {
                ItemId = itemId,
                QuestId = x.QuestId
            }));
        }

        ctx.SaveChanges();
        _logger.Info($"{counter} etc items saved");
    }

    private static IconType GetIconTypeFromBodyPart(ItemBodyPart bodyPart) =>
        bodyPart switch
        {
            ItemBodyPart.OnePiece => IconType.OnePiece,
            ItemBodyPart.Chest => IconType.UpperBody,
            ItemBodyPart.Legs => IconType.LowerBody,
            ItemBodyPart.Feet => IconType.Boots,
            ItemBodyPart.Gloves => IconType.Gloves,
            ItemBodyPart.Head => IconType.Helmet,
            ItemBodyPart.REar => IconType.Accessary,
            ItemBodyPart.RFinger => IconType.Accessary,
            ItemBodyPart.Neck => IconType.Accessary,
            ItemBodyPart.Hair => IconType.Accessary,
            ItemBodyPart.LHand => IconType.Shield,
            ItemBodyPart.LrHand => IconType.Weapon,
            ItemBodyPart.RHand => IconType.Weapon,
            ItemBodyPart.RHand1 => IconType.Accessary,
            ItemBodyPart.RBracelet => IconType.Accessary,
            ItemBodyPart.LBracelet => IconType.Accessary,
            ItemBodyPart.HairAll => IconType.Accessary,
            ItemBodyPart.Waist => IconType.Accessary,
            ItemBodyPart.Back => IconType.None,
            ItemBodyPart.Deco1 => IconType.Etc,
            ItemBodyPart.AgathionMain => IconType.Etc,
            ItemBodyPart.Underwear => IconType.Etc,
            ItemBodyPart.Jewel1 => IconType.Etc,
            ItemBodyPart.Brooch => IconType.Etc,
            _ => throw new ArgumentOutOfRangeException(nameof(bodyPart), bodyPart, "Not supported")
        };
}