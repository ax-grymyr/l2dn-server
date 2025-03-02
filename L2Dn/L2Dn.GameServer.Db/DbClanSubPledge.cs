using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(ClanId), nameof(SubPledgeId))]
[Index(nameof(LeaderId), IsUnique = true)]
public class DbClanSubPledge
{
    public int ClanId { get; set; }

    [ForeignKey(nameof(ClanId))]
    public DbClan Clan { get; set; } = null!;

    public int SubPledgeId { get; set; } = -2;

    [MaxLength(45)]
    public string Name { get; set; } = string.Empty;

    public int LeaderId { get; set; }
    [ForeignKey(nameof(LeaderId))]

    public DbCharacter Leader { get; set; } = null!;
}