using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(ClanId))]
public class DbPledgeApplicant
{
    public int CharacterId { get; set; }
    public int ClanId { get; set; }
    public int Karma { get; set; }

    [MaxLength(255)]
    public string Message { get; set; } = string.Empty;
}