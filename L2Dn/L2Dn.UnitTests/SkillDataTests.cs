using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using L2Dn.IO;
using L2Dn.Packages.DatDefinitions;
using L2Dn.Packages.DatDefinitions.Definitions;

namespace L2Dn;

public class SkillDataTests
{
    [Fact]
    public void CompareSkills()
    {
        // The test compares skill names from EU client for 447 protocol and
        // from Classic 3.0 data pack (228 protocol).
        
        const string euPath = @"D:\L2\L2EU-P447-D20240313-P-230809-240318-1\system\eu\";
        const string classic30DataPackPath = @"D:\L2\L2J_Mobius_Classic_3.0_TheKamael\game\data\stats\skills"; 

        // Load skills from EU 447 client
        EncryptionKeys.RsaDecryption413 = EncryptionKeys.RsaDecryption413L2EncDec;
        SkillNameV6 euSkillName = DatReader.Read<SkillNameV6>(euPath + "SkillName_Classic-eu.dat");
        
        // Strings dictionary
        Dictionary<int, string> strings = euSkillName.Texts.ToDictionary(x => (int)x.Index, x => x.Text);

        // Skills
        string GetString(int index) => index >= 0 ? strings[index].Trim() : string.Empty;
        List<Skill> skills = euSkillName.Records.Select(x => new Skill((int)x.SkillId, x.SkillLevel, x.SkillSubLevel,
                GetString(x.Name), GetString(x.Description),
                GetString(x.DescriptionParam), GetString(x.EnchantName), GetString(x.EnchantNameParam),
                GetString(x.EnchantDesc), GetString(x.EnchantDescParam)))
            .OrderBy(x => x.Id).ThenBy(x => x.Level).ThenBy(x => x.SubLevel)
            .ToList();
        
        Serialize(@"D:\eu-skills.json", skills);
        
        // Load skills from Classic 3.0 data pack
        List<XmlSkill> xmlSkills = Directory
            .EnumerateFiles(classic30DataPackPath, "*.xml", SearchOption.AllDirectories)
            .SelectMany(file => XDocument.Load(file).Elements("list").Elements("skill")).Select(el =>
                new XmlSkill((int)el.Attribute("id"), (int)el.Attribute("toLevel"), ((string)el.Attribute("name")).Trim()))
            .OrderBy(x => x.Id)
            .ToList();
        
        // Compare skill names and write the results
        using FileStream stream = new FileStream(@"D:\skills.txt", FileMode.Create, FileAccess.Write, FileShare.None);
        using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
        var ids = xmlSkills.Select(x => x.Id).Concat(skills.Select(x => x.Id)).Distinct().Order();
        foreach (int id in ids)
        {
            XmlSkill? xmlSkill = xmlSkills.SingleOrDefault(x => x.Id == id);
            List<Skill> euSkills = skills.Where(x => x.Id == id).ToList();

            if (xmlSkill is null)
            {
                if (euSkills.Count == 0)
                    continue;
                
                writer.WriteLine(
                    $"Skill id={id}, name='{euSkills[0].Name}' from EU client not found in Classic 3.0 data pack");
                
                continue;
            }
            
            // if (euSkills.Count == 0)
            // {
            //     writer.WriteLine(
            //         $"Skill id={xmlSkill.Id}, name='{xmlSkill.Name}' from Classic 3.0 data pack not found in EU client");
            // }

            foreach (Skill skill in euSkills)
            {
                if (!string.Equals(xmlSkill.Name, skill.Name, StringComparison.OrdinalIgnoreCase))
                {
                    writer.WriteLine(
                        $"Skill id={xmlSkill.Id}, name='{xmlSkill.Name}' has different name in EU client name='{skill.Name}', level={skill.Level}, subLevel={skill.SubLevel}");
                }

                if (skill.Level > xmlSkill.MaxLevel)
                {
                    writer.WriteLine(
                        $"Skill id={xmlSkill.Id}, name='{xmlSkill.Name}' has greater level in EU client ({skill.Level}) than max level in Classic 3.0 data pack ({xmlSkill.MaxLevel})");
                }
            }
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

    private record XmlSkill(int Id, int MaxLevel, string Name);
    
    private record Skill(
        int Id,
        int Level,
        int SubLevel,
        string Name,
        string Description,
        string DescriptionParam,
        string EnchantName,
        string EnchantNameParam,
        string EnchantDescription,
        string EnchantDescriptionParam);
}