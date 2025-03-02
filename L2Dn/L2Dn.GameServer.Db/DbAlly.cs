using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[Table("Allys")] // for compatibility
[Index(nameof(Name), IsUnique = true)]
public class DbAlly
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(70)]
    public string Name { get; set; } = string.Empty;

    public int? CrestId { get; set; }

    [ForeignKey(nameof(CrestId))]
    public DbCrest? Crest { get; set; }
}