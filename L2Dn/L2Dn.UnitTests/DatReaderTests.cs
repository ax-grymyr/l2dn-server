using System.Text.Encodings.Web;
using System.Text.Json;
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

        string srcDataNamePath = Path.Combine(srcPath, "L2GameDataName.dat");
        string dstDataNamePath = Path.Combine(dstPath, $"L2GameDataName-{langDstSuffix}.dat");
        
        string srcItemNamePath = Path.Combine(srcPath, $"ItemName_Classic-{langSuffix}.dat");
        string dstItemNamePath = Path.Combine(dstPath, $"ItemName_Classic-{langDstSuffix}.NoNames.json");
        string dstItemNamePath2 = Path.Combine(dstPath, $"ItemName_Classic-{langDstSuffix}.json");

        string srcQuestNamePath = Path.Combine(srcPath, $"QuestName_Classic-{langSuffix}.dat");
        string dstQuestNamePath = Path.Combine(dstPath, $"QuestName_Classic-{langDstSuffix}.json");

        string srcSkillNamePath = Path.Combine(srcPath, $"SkillName_Classic-{langSuffix}.dat");
        string dstSkillNamePath = Path.Combine(dstPath, $"SkillName_Classic-{langDstSuffix}.json");

        string srcTeleportListPath = Path.Combine(srcPath, "teleportlist_Classic.dat");
        string dstTeleportListPath = Path.Combine(dstPath, $"teleportlist_Classic-{langDstSuffix}.json");

        string srcTutorialNamePath = Path.Combine(srcPath, $"TutorialName_Classic-{langSuffix}.dat");
        string dstTutorialNamePath = Path.Combine(dstPath, $"TutorialName_Classic-{langDstSuffix}.json");
        
        string srcZoneNamePath = Path.Combine(srcPath, $"ZoneName_Classic-{langSuffix}.dat");
        string dstZoneNamePath = Path.Combine(dstPath, $"ZoneName_Classic-{langDstSuffix}.NoNames.json");
        string dstZoneNamePath2 = Path.Combine(dstPath, $"ZoneName_Classic-{langDstSuffix}.json");
        
        ItemNameV18 itemName = DatReader.Read<ItemNameV18>(srcItemNamePath);
        Serialize(dstItemNamePath, itemName);

        SkillNameV6 skillName = DatReader.Read<SkillNameV6>(srcSkillNamePath);
        Serialize(dstSkillNamePath, skillName);
        
        TeleportListV3 teleportList = DatReader.Read<TeleportListV3>(srcTeleportListPath);
        Serialize(dstTeleportListPath, teleportList);
        
        TutorialNameV2 tutorialName = DatReader.Read<TutorialNameV2>(srcTutorialNamePath);
        Serialize(dstTutorialNamePath, tutorialName);
        
        ZoneNameV7 zoneName = DatReader.Read<ZoneNameV7>(srcZoneNamePath);
        Serialize(dstZoneNamePath, zoneName);
        
        // loading strings
        L2NameData names = DatReader.ReadNameData(srcDataNamePath);
        Serialize(dstDataNamePath, names.Names);

        itemName = DatReader.Read<ItemNameV18>(srcItemNamePath);
        Serialize(dstItemNamePath2, itemName);
        
        QuestNameV8 questName = DatReader.Read<QuestNameV8>(srcQuestNamePath);
        Serialize(dstQuestNamePath, questName);

        zoneName = DatReader.Read<ZoneNameV7>(srcZoneNamePath);
        Serialize(dstZoneNamePath2, zoneName);
    }
    
    private static void Serialize<T>(string filePath, T obj)
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;
        options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

        using FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        JsonSerializer.Serialize(stream, obj, options);
    }
}