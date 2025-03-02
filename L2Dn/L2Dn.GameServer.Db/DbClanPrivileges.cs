using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(ClanId), nameof(Rank), nameof(Party))]
public class DbClanPrivileges
{
    public int ClanId { get; set; }

    [ForeignKey(nameof(ClanId))]
    public DbClan Clan { get; set; } = null!;

    public int Rank { get; set; }
    public int Party { get; set; }
    public int Privileges { get; set; }
}