using System.Text;
using System.Text.Json;
using L2Dn.IO;
using L2Dn.Packages.DatDefinitions;
using L2Dn.Packages.DatDefinitions.Fafurion;

namespace L2Dn;

public class DatReaderTests
{
    [Fact]
    public void ReadQuests()
    {
        EncryptionKeys.RsaDecryption413 = EncryptionKeys.RsaDecryption413L2EncDec; 
        
        QuestName quests =
            DatReader.Read<QuestName>(
                @"D:\L2\L2EU-P447-D20240205-P-230809-240305-1\system\eu\QuestName_Classic-eu.dat");

        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;
        string json = JsonSerializer.Serialize(quests, options);
        File.WriteAllText(@"D:\Quests.json", json, Encoding.UTF8);
    }
}