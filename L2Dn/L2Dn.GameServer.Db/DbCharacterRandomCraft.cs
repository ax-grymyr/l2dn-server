using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class DbCharacterRandomCraft
{
    [Key]
    public int CharacterId { get; set; }

    public int FullPoints { get; set; }
    public int Points { get; set; }
    public bool IsSayhaRoll { get; set; }

    public int Item1Id { get; set; }
    public long Item1Count { get; set; }
    public bool Item1Locked { get; set; }
    public int Item1LockLeft { get; set; }

    public int Item2Id { get; set; }
    public long Item2Count { get; set; }
    public bool Item2Locked { get; set; }
    public int Item2LockLeft { get; set; }

    public int Item3Id { get; set; }
    public long Item3Count { get; set; }
    public bool Item3Locked { get; set; }
    public int Item3LockLeft { get; set; }

    public int Item4Id { get; set; }
    public long Item4Count { get; set; }
    public bool Item4Locked { get; set; }
    public int Item4LockLeft { get; set; }

    public int Item5Id { get; set; }
    public long Item5Count { get; set; }
    public bool Item5Locked { get; set; }
    public int Item5LockLeft { get; set; }
}