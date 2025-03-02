using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(OwnerId), nameof(SummonId), nameof(SummonSkillId))]
public class DbCharacterSummon
{
    public int OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public DbCharacter Owner { get; set; } = null!;

    public int SummonId { get; set; }
    public int SummonSkillId { get; set; }
    public int CurrentHp { get; set; }
    public int CurrentMp { get; set; }
    public TimeSpan? Time { get; set; }
}