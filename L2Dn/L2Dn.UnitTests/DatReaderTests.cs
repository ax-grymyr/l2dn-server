using System.Text;
using System.Text.Json;
using L2Dn.IO;
using L2Dn.Packages.DatDefinitions;
using L2Dn.Packages.DatDefinitions.Fafurion;
using L2Dn.Packages.DatDefinitions.Shinemaker;

namespace L2Dn;

public class DatReaderTests
{
    private const string SourcePath = @"D:\L2\L2EU-P447-D20240313-P-230809-240318-1\system\eu\";
    private const string DestPath = SourcePath;
    
    [Fact]
    public void ReadQuests()
    {
        EncryptionKeys.RsaDecryption413 = EncryptionKeys.RsaDecryption413L2EncDec;

        QuestName[] quests = DatReader.ReadArray<QuestName>(SourcePath + "QuestName_Classic-eu.dat");
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;
        string json = JsonSerializer.Serialize(quests, options);
        File.WriteAllText(DestPath + "QuestName_Classic-eu.json", json, Encoding.UTF8);
    }

    [Fact]
    public void ReadItems()
    {
        EncryptionKeys.RsaDecryption413 = EncryptionKeys.RsaDecryption413L2EncDec;

        DatReader.ReadNameData(SourcePath + "L2GameDataName.dat");
        Items items = DatReader.Read<Items>(SourcePath + "ItemName_Classic-eu.dat");
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;
        string json = JsonSerializer.Serialize(items, options);
        File.WriteAllText(DestPath + "ItemName_Classic-eu.json", json, Encoding.UTF8);
    }

    [Fact]
    public void ReadNames()
    {
        EncryptionKeys.RsaDecryption413 = EncryptionKeys.RsaDecryption413L2EncDec;

        L2NameData names = DatReader.Read<L2NameData>(SourcePath + "L2GameDataName.dat");
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;
        string json = JsonSerializer.Serialize(names, options);
        File.WriteAllText(DestPath + "L2GameDataName.json", json, Encoding.UTF8);
    }
}