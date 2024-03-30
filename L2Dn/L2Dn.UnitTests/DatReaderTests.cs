using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        QuestName[] quests = DatReader.ReadArray<QuestName>(SourcePath + "QuestName_Classic-eu.dat");
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

    private static void ConvertDatFiles(string srcPath, string dstPath, string langSuffix, string langDstSuffix)
    {
        DatReader.ClearNameData();

        DatConversion[] conversions = 
        [
            //new DatConversion<AbnormalAgathion>("AbnormalAgathion", "AbnormalAgathion-{0}", false),
            //new DatConversion<AbnormalDefaultEffectV5>("AbnormalDefaultEffect", "AbnormalDefaultEffect-{0}", true),
            //new DatConversion<AbnormalEdgeEffectDataV2>("AbnormalEdgeEffectData", "AbnormalEdgeEffectData-{0}", false),
            //new DatConversion<ActionNameV4>("ActionName_Classic-{0}", "ActionName_Classic-{0}", true),
            //new DatConversion<AdditionalEffectV7>("AdditionalEffect_Classic", "AdditionalEffect_Classic-{0}", true),
            //new DatConversion<AdditionalItemGrpV4>("AdditionalItemGrp_Classic", "AdditionalItemGrp_Classic-{0}", false),
            //new DatConversion<AdditionalJewelEquipEffect>("AdditionalJewelEquipEffect_Classic", "AdditionalJewelEquipEffect-{0}", false),
            //new DatConversion<AdditionalNpcGrpPartsV4>("AdditionalNpcGrpParts_Classic", "AdditionalNpcGrpParts_Classic-{0}", false),
            //new DatConversion<AdditionalSoulshotEffectV2>("AdditionalSoulshotEffect_Classic", "AdditionalSoulshotEffect_Classic-{0}", false),
            //new DatConversion<AgathionData>("agathiondata_Classic", "AgathionData_Classic-{0}", false),
            //new DatConversion<ArmorGrpV14>("Armorgrp_Classic", "ArmorGrp_Classic-{0}", true),
            //new DatConversion<BlessOptionV2>("BlessOption_Classic", "BlessOption_Classic-{0}", false),
            //new DatConversion<CardCollectData>("CardCollectData_Classic-{0}", "CardCollectData_Classic-{0}", true),
            //new DatConversion<CastleNameV4>("CastleName_Classic-{0}", "CastleName_Classic-{0}", true),
            //new DatConversion<CharacterAbilityV2>("CharacterAbility_Classic-{0}", "CharacterAbility_Classic-{0}", true),
            //new DatConversion<CharacterInitialStatExData>("CharacterInitialStatExData", "CharacterInitialStatExData-{0}", false),
            //new DatConversion<CharCreateGrpV10>("CharCreategrp_Classic", "CharCreateGrp_Classic-{0}", true),
            new DatConversion<CollectionV2>("collection_Classic-{0}", "Collection_Classic-{0}", false),
            //new DatConversion<ServerNameV5>("ServerName-{0}", "ServerName-{0}", false),
            //new DatConversion<NpcName>("NpcName_Classic-{0}", "NpcName_Classic-{0}", false),
            //new DatConversion<NpcString>("NpcString_Classic-{0}", "NpcString_Classic-{0}", false),
            //new DatConversion<NpcTeleporter>("NPCTeleporter_Classic", "NpcTeleporter_Classic-{0}", false),
            //new DatConversion<ItemNameV18>("ItemName_Classic-{0}", "ItemName_Classic-{0}", true),
            //new DatConversion<QuestNameV8>("QuestName_Classic-{0}", "QuestName_Classic-{0}", false),
            //new DatConversion<SkillNameV6>("SkillName_Classic-{0}", "SkillName_Classic-{0}", false),
            //new DatConversion<TeleportListV3>("teleportlist_Classic", "TeleportList_Classic-{0}", false),
            //new DatConversion<TutorialNameV2>("TutorialName_Classic-{0}", "TutorialName_Classic-{0}", false),
            //new DatConversion<ZoneNameV7>("ZoneName_Classic-{0}", "ZoneName_Classic-{0}", false),
            //new DatConversion<SysString>("SysString_Classic-{0}", "SysString_Classic-{0}", false),
            //new DatConversion<SystemMsgV6>("SystemMsg_Classic-{0}", "SystemMsg_Classic-{0}", true),
            //new DatConversion<StaticObjectV2>("StaticObject_Classic-{0}", "StaticObject_Classic-{0}", true),
            //new DatConversion<SteadyBox>("SteadyBox_Classic-{0}", "SteadyBox_Classic-{0}", false),
            //new DatConversion<Subjugation>("Subjugation_Classic-{0}", "Subjugation_Classic-{0}", false),
        ];

        foreach (DatConversion conversion in conversions)
        {
            string srcFilePath = Path.Combine(srcPath, string.Format(conversion.SrcFile, langSuffix) + ".dat");
            string suffix = conversion.UseDataNames ? ".NoNames.json" : ".json";
            string dstFilePath = Path.Combine(dstPath, string.Format(conversion.DstFile, langDstSuffix) + suffix);
            object obj = DatReader.Read(conversion.ObjectType, srcFilePath);
            Serialize(dstFilePath, obj);
        }

        string srcDataNamePath = Path.Combine(srcPath, "L2GameDataName.dat");
        string dstDataNamePath = Path.Combine(dstPath, $"L2GameDataName-{langDstSuffix}.dat");
        
        // loading strings
        L2NameData names = DatReader.ReadNameData(srcDataNamePath);
        Serialize(dstDataNamePath, names.Names);

        foreach (DatConversion conversion in conversions)
        {
            if (conversion.UseDataNames)
            {
                string srcFilePath = Path.Combine(srcPath, string.Format(conversion.SrcFile, langSuffix) + ".dat");
                string dstFilePath = Path.Combine(dstPath, string.Format(conversion.DstFile, langDstSuffix) + ".json");
                object obj = DatReader.Read(conversion.ObjectType, srcFilePath);
                Serialize(dstFilePath, obj);
            }
        }
    }
    
    private static void Serialize<T>(string filePath, T obj)
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;
        options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        options.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;

        using FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        JsonSerializer.Serialize(stream, obj, options);
    }

    private record DatConversion(Type ObjectType, string SrcFile, string DstFile, bool UseDataNames);

    private record DatConversion<T>(string SrcFile, string DstFile, bool UseDataNames)
        : DatConversion(typeof(T), SrcFile, DstFile, UseDataNames);
}