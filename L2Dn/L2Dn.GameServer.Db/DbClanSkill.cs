using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(ClanId), nameof(SkillId), nameof(SubPledgeId))]
public class DbClanSkill
{
    public int ClanId { get; set; }

    [ForeignKey(nameof(ClanId))]
    public DbClan Clan { get; set; } = null!;

    public int SkillId { get; set; }
    public short SkillLevel { get; set; }

    [MaxLength(26)]
    public string SkillName { get; set; } = string.Empty;

    public int SubPledgeId { get; set; } = -2;
}