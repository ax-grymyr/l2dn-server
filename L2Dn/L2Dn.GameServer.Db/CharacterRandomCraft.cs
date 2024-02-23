using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class CharacterRandomCraft
{
    [Key]
    public int CharacterId { get; set; }

    public int FullPoints { get; set; }
    public int Points { get; set; }
    public bool IsSayhaRoll { get; set; }

    public CharacterRandomCraftItem Item1 { get; set; }
    public CharacterRandomCraftItem Item2 { get; set; }
    public CharacterRandomCraftItem Item3 { get; set; }
    public CharacterRandomCraftItem Item4 { get; set; }
    public CharacterRandomCraftItem Item5 { get; set; }
}