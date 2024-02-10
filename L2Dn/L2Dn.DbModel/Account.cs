using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.DbModel;

[Index(nameof(Login), IsUnique = true)]
public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
 
    //[Required]
    [MaxLength(40)]
    public string Login { get; set; } = string.Empty;

    [MaxLength(255)]
    public string EMail { get; set; } = string.Empty;

    //[Required]
    [MaxLength(32)]
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
 
    public DateTime Created { get; set; }
    public DateTime LastLoginTime { get; set; }
    
    [MaxLength(45)]
    public string? LastIpAddress { get; set; }
    
    public byte? LastSelectedServerId { get; set; }
}