using System.Text.Encodings.Web;
using System.Text.Json;
using L2Dn.IO;
using L2Dn.Packages.DatDefinitions;
using L2Dn.Packages.DatDefinitions.Fafurion;
using L2Dn.Packages.DatDefinitions.Shinemaker;

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
        Items items = DatReader.Read<Items>(SourcePath + "ItemName_Classic-eu.dat");
        Serialize(DestPath + "ItemName_Classic-eu.json", items);
    }

    [Fact]
    public void ReadNames()
    {
        EncryptionKeys.RsaDecryption413 = EncryptionKeys.RsaDecryption413L2EncDec;

        L2NameData names = DatReader.Read<L2NameData>(SourcePath + "L2GameDataName.dat");
        Serialize(DestPath + "L2GameDataName.json", names.Names);
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