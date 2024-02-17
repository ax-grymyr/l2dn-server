using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(AccountId), nameof(Name))]
public class AccountVariable
{
    public int AccountId { get; set; }

    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Value { get; set; }
}