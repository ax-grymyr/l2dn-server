using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[Index(nameof(Login), IsUnique = true)]
public class AccountRef
{
    public int Id { get; set; }

    [MaxLength(40)]
    public string Login { get; set; } = string.Empty;

    public DateTime? LastLogin { get; set; }

    [MaxLength(45)]
    public string? LastIpAddress { get; set; }
}