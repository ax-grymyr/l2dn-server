using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.AuthServer.Db;

[Index(nameof(Login), IsUnique = true)]
public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
 
    [MaxLength(40)]
    public string Login { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? EMail { get; set; }

    [MaxLength(32)]
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
 
    public DateTime Created { get; set; }
    public DateTime? LastLogin { get; set; }
    
    [MaxLength(45)]
    public string? LastIpAddress { get; set; }
    
    public byte? LastSelectedServerId { get; set; }
}