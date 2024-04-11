using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using L2Dn.IO;
using L2Dn.Packages.DatDefinitions;
using L2Dn.Packages.DatDefinitions.Definitions;

namespace L2Dn;

public class DatReaderTests
{
    private const string SourcePath = @"D:\L2\L2EU-P447-D20240313-P-230809-240318-1\system\eu\";
    private const string DestPath = @"D:\L2\L2EU-P447-D20240313-P-230809-240318-1\";
    
    [Fact]
    public void ReadQuests()
    {
        EncryptionKeys.RsaDecryption413 = EncryptionKeys.RsaDecryption413L2EncDec;

        QuestName quests = DatReader.Read<QuestName>(SourcePath + "QuestName_Classic-eu.dat");
        Serialize(DestPath + "QuestName_Classic-eu.json", quests);
    }

    [Fact]
    public void ReadItems()
    {
        EncryptionKeys.RsaDecryption413 = EncryptionKeys.RsaDecryption413L2EncDec;

        DatReader.ReadNameData(SourcePath + "L2GameDataName.dat");
        ItemNameV18 items = DatReader.Read<ItemNameV18>(SourcePath + "ItemName_Classic-eu.dat");
        Serialize(DestPath + "ItemName_Classic-eu.json", items);
    }

    [Fact]
    public void ReadNames()
    {
        EncryptionKeys.RsaDecryption413 = EncryptionKeys.RsaDecryption413L2EncDec;

        L2NameData names = DatReader.Read<L2NameData>(SourcePath + "L2GameDataName.dat");
        Serialize(DestPath + "L2GameDataName.json", names.Names);
    }

    [Fact]
    public void ReplaceRuItemNamesInEuClient()
    {
        const string euPath = @"D:\L2\L2EU-P447-D20240313-P-230809-240318-1\system\eu\";
        const string euOldPath = @"D:\L2\L2J_Mobius_Classic_3.0 - The Kamael P228 Client\system\";
        const string naPath = @"D:\L2\L2NA-P447-D20240320-G458-230809.72\system\";

        EncryptionKeys.RsaDecryption413 = EncryptionKeys.RsaDecryption413L2EncDec;

        // Load EU item names from old EU 228 client
        L2NameData euOldNameData = DatReader.ReadNameData(euOldPath + "L2GameDataName.dat");
        //Serialize(@"D:\L2\DatFiles\L2GameDataName-eu-P228.json", euOldNameData);
        ItemNameV11 euOldItemName = DatReader.Read<ItemNameV11>(euOldPath + "ItemName_Classic-eu.dat");
        //Serialize(@"D:\L2\DatFiles\ItemName_Classic-eu-P228.json", euOldItemName);

        // Load EU item names (EU client has Russian names for items for some reason)
        L2NameData euNameData = DatReader.ReadNameData(euPath + "L2GameDataName.dat.original");
        //Serialize(@"D:\L2\DatFiles\L2GameDataName-eu.json", euNameData);
        ItemNameV18 euItemName = DatReader.Read<ItemNameV18>(euPath + "ItemName_Classic-eu.dat.original");
        //Serialize(@"D:\L2\DatFiles\ItemName_Classic-eu.json", euItemName);
        
        // Load NA item names
        DatReader.ReadNameData(naPath + "L2GameDataName.dat");
        ItemNameV18 naItemName = DatReader.Read<ItemNameV18>(naPath + "ItemName_Classic-e.dat");
        
        // Replace item names
        bool euNameDataUpdateNeeded = false;
        List<string> euStrings = euNameData.Names.ToList();
        Dictionary<string, int> euStringIndexMap =
            euStrings.Select((s, i) => new KeyValuePair<string, int>(s, i))
                .ToDictionary(StringComparer.OrdinalIgnoreCase);
        
        foreach (ItemNameV18.ItemNameRecord euItemNameRecord in euItemName.Records)
        {
            uint itemId = euItemNameRecord.Id;
            ItemNameV11.ItemNameRecord? euOldItemNameRecord = euOldItemName.Records.FirstOrDefault(x => x.Id == itemId);
            ItemNameV18.ItemNameRecord? naItemNameRecord = naItemName.Records.FirstOrDefault(x => x.Id == itemId);
            if (euOldItemNameRecord != null)
            {
                // Replace name from EU client
                string newName = euOldItemNameRecord.Name.Text;
                if (!euStringIndexMap.TryGetValue(newName, out int index))
                {
                    index = euStrings.Count;
                    euStrings.Add(newName);
                    euStringIndexMap.Add(newName, index);
                    euNameDataUpdateNeeded = true;
                }
                
                euItemNameRecord.Name = new IndexedString(newName, index);
                euItemNameRecord.AdditionalName = euOldItemNameRecord.AdditionalName;
                euItemNameRecord.Description = euOldItemNameRecord.Description;
            }
            else if (naItemNameRecord != null)
            {
                // Replace name from NA client
                string newName = naItemNameRecord.Name.Text;
                if (!euStringIndexMap.TryGetValue(newName, out int index))
                {
                    index = euStrings.Count;
                    euStrings.Add(newName);
                    euStringIndexMap.Add(newName, index);
                    euNameDataUpdateNeeded = true;
                }

                euItemNameRecord.Name = new IndexedString(newName, index);
                euItemNameRecord.AdditionalName = naItemNameRecord.AdditionalName;
                euItemNameRecord.Description = naItemNameRecord.Description;
            }
        }

        if (euNameDataUpdateNeeded)
        {
            euNameData.Names = euStrings.ToArray();
            
            // Write name data
            DatWriter.Write(euPath + "L2GameDataName.dat.modified", euNameData);

            // Verify output file
            using FileStream stream1 = File.OpenRead(euPath + "L2GameDataName.dat.modified");
            L2NameData euTestNameData = DatReader.Read<L2NameData>(stream1);
            euNameData.Names.Should().BeEquivalentTo(euTestNameData.Names);
            DatReader.SetNameData(euTestNameData.Names);
            Serialize(@"D:\L2\DatFiles\L2GameDataName.modified.json", euTestNameData);
        }
        
        // Write items
        DatWriter.Write(euPath + "ItemName_Classic-eu.dat.modified", euItemName);
        
        // Verify output file
        using FileStream stream = File.OpenRead(euPath + "ItemName_Classic-eu.dat.modified");
        ItemNameV18 euTestItemName = DatReader.Read<ItemNameV18>(stream);
        //euTestItemName.Should().BeEquivalentTo(euItemName);
        Serialize(@"D:\L2\DatFiles\ItemName_Classic-eu.modified.json", euTestItemName);
    }
    
    [Fact]
    public void ConvertDataFiles()
    {
        EncryptionKeys.RsaDecryption413 = EncryptionKeys.RsaDecryption413L2EncDec;

        const string dstPath = @"D:\L2\DatFiles\";
        
        const string euPath = @"D:\L2\L2EU-P447-D20240313-P-230809-240318-1\system\eu\";
        const string ruPath = @"D:\L2\CLRU-P447-D20240313-P-230809-240318-1\system\";
        const string naPath = @"D:\L2\L2NA-P447-D20240320-G458-230809.72\system\";

        // EU
        ConvertDatFiles(euPath, dstPath, "eu", "eu");
        
        // RU
        ConvertDatFiles(ruPath, dstPath, "ru", "ru");
        
        // NA
        ConvertDatFiles(naPath, dstPath, "e", "na");
    }
    
    [Fact]
    public void CheckModifiedFiles()
    {
        EncryptionKeys.RsaDecryption413 = EncryptionKeys.RsaDecryption413L2EncDec;

        const string dstPath = @"D:\L2\DatFiles\";
        const string euPath = @"D:\L2\L2EU-P447-D20240313-P-230809-240318-1\system\eu\";

        ItemNameV18 euItemName = DatReader.Read<ItemNameV18>(euPath + "ItemName_Classic-eu.dat-modified-encrypted");
        Serialize(dstPath + "ItemName_Classic-eu.modified.NoNames.json", euItemName);
        L2NameData euNameData = DatReader.ReadNameData(euPath + "L2GameDataName.dat.modified-encrypted");
        Serialize(dstPath + "L2GameDataName.dat.modified.json", euNameData);
        euItemName = DatReader.Read<ItemNameV18>(euPath + "ItemName_Classic-eu.dat-modified-encrypted");
        Serialize(dstPath + "ItemName_Classic-eu.modified.json", euItemName);
    }

    private static void ConvertDatFiles(string srcPath, string dstPath, string langSuffix, string langDstSuffix)
    {
        DatConversion[] conversions = 
        [
            new DatConversion<AbnormalAgathion>("AbnormalAgathion", "AbnormalAgathion-{0}"),
            new DatConversion<AbnormalDefaultEffectV5>("AbnormalDefaultEffect", "AbnormalDefaultEffect-{0}"),
            new DatConversion<AbnormalEdgeEffectDataV2>("AbnormalEdgeEffectData", "AbnormalEdgeEffectData-{0}"),
            new DatConversion<ActionNameV4>("ActionName_Classic-{0}", "ActionName_Classic-{0}"),
            new DatConversion<AdditionalEffectV7>("AdditionalEffect_Classic", "AdditionalEffect_Classic-{0}"),
            new DatConversion<AdditionalItemGrpV4>("AdditionalItemGrp_Classic", "AdditionalItemGrp_Classic-{0}"),
            new DatConversion<AdditionalJewelEquipEffect>("AdditionalJewelEquipEffect_Classic", "AdditionalJewelEquipEffect-{0}"),
            new DatConversion<AdditionalNpcGrpPartsV4>("AdditionalNpcGrpParts_Classic", "AdditionalNpcGrpParts_Classic-{0}"),
            new DatConversion<AdditionalSoulshotEffectV2>("AdditionalSoulshotEffect_Classic", "AdditionalSoulshotEffect_Classic-{0}"),
            new DatConversion<AgathionData>("agathiondata_Classic", "AgathionData_Classic-{0}"),
            new DatConversion<ArmorGrpV14>("Armorgrp_Classic", "ArmorGrp_Classic-{0}"),
            new DatConversion<BlessOptionV2>("BlessOption_Classic", "BlessOption_Classic-{0}"),
            new DatConversion<CardCollectData>("CardCollectData_Classic-{0}", "CardCollectData_Classic-{0}"),
            new DatConversion<CastleNameV4>("CastleName_Classic-{0}", "CastleName_Classic-{0}"),
            new DatConversion<CharacterAbilityV2>("CharacterAbility_Classic-{0}", "CharacterAbility_Classic-{0}"),
            new DatConversion<CharacterInitialStatExData>("CharacterInitialStatExData", "CharacterInitialStatExData-{0}"),
            //new DatConversion<CharCreateGrpV10>("CharCreategrp_Classic", "CharCreateGrp_Classic-{0}", true), // Not Valid
            new DatConversion<CollectionV2>("collection_Classic-{0}", "Collection_Classic-{0}"),
            new DatConversion<CollectionMain>("collectionmain_Classic", "CollectionMain_Classic-{0}"),
            new DatConversion<CombinationItemDataV7>("CombinationItemData_Classic", "CombinationItemData_Classic-{0}"),
            new DatConversion<CommandNameV2>("CommandName_Classic-{0}", "CommandName_Classic-{0}"),
            new DatConversion<CommonLook>("CommonLook_Classic", "CommonLook_Classic-{0}"),
            new DatConversion<Costume>("Costume_Classic", "Costume_Classic-{0}"),
            
            new DatConversion<EtcItemGrpV9>("EtcItemgrp_Classic", "EtcItemGrp_Classic-{0}"),
            new DatConversion<ItemNameV18>("ItemName_Classic-{0}", "ItemName_Classic-{0}"),
            new DatConversion<LCoinShopProductV6>("LCoinShopProduct_Classic-{0}", "LCoinShopProduct_Classic-{0}"),
            new DatConversion<NpcName>("NpcName_Classic-{0}", "NpcName_Classic-{0}"),
            new DatConversion<NpcString>("NpcString_Classic-{0}", "NpcString_Classic-{0}"),
            new DatConversion<NpcTeleporter>("NPCTeleporter_Classic", "NpcTeleporter_Classic-{0}"),
            new DatConversion<QuestNameV8>("QuestName_Classic-{0}", "QuestName_Classic-{0}"),
            new DatConversion<ServerNameV5>("ServerName-{0}", "ServerName-{0}"),
            new DatConversion<SetItemGrp>("SetItemGrp_Classic-{0}", "SetItemGrp_Classic-{0}"),
            new DatConversion<SkillNameV6>("SkillName_Classic-{0}", "SkillName_Classic-{0}"),
            new DatConversion<StaticObjectV2>("StaticObject_Classic-{0}", "StaticObject_Classic-{0}"),
            new DatConversion<SysString>("SysString_Classic-{0}", "SysString_Classic-{0}"),
            new DatConversion<SystemMsgV6>("SystemMsg_Classic-{0}", "SystemMsg_Classic-{0}"),
            new DatConversion<SteadyBox>("SteadyBox_Classic-{0}", "SteadyBox_Classic-{0}"),
            new DatConversion<Subjugation>("Subjugation_Classic-{0}", "Subjugation_Classic-{0}"),
            new DatConversion<TeleportListV3>("teleportlist_Classic", "TeleportList_Classic-{0}"),
            new DatConversion<TutorialNameV2>("TutorialName_Classic-{0}", "TutorialName_Classic-{0}"),
            new DatConversion<WeaponGrpV12>("Weapongrp_Classic", "WeaponGrp_Classic-{0}"),
            new DatConversion<ZoneNameV7>("ZoneName_Classic-{0}", "ZoneName_Classic-{0}"),
        ];

        // loading strings
        string srcDataNamePath = Path.Combine(srcPath, "L2GameDataName.dat");
        string dstDataNamePath = Path.Combine(dstPath, $"L2GameDataName-{langDstSuffix}.json");
        L2NameData names = DatReader.ReadNameData(srcDataNamePath);
        Serialize(dstDataNamePath, names.Names);

        foreach (DatConversion conversion in conversions)
        {
            string srcFilePath = Path.Combine(srcPath, string.Format(conversion.SrcFile, langSuffix) + ".dat");
            string dstFilePath = Path.Combine(dstPath, string.Format(conversion.DstFile, langDstSuffix) + ".json");
            object obj = DatReader.Read(conversion.ObjectType, srcFilePath);
            Serialize(dstFilePath, obj);
        }
    }

    private static void Serialize<T>(string filePath, T obj)
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;
        options.Converters.Add(new IndexedStringConverter());
        options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        options.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;

        using FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        JsonSerializer.Serialize(stream, obj, options);
    }

    private record DatConversion(Type ObjectType, string SrcFile, string DstFile);
    private record DatConversion<T>(string SrcFile, string DstFile): DatConversion(typeof(T), SrcFile, DstFile);
}