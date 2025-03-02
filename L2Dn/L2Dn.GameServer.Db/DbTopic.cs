using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace L2Dn.GameServer.Db;

public class DbTopic
{
    [Key]
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int ForumId { get; set; }

    [ForeignKey(nameof(ForumId))]
    public DbForum Forum { get; set; } = null!;

    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    [MaxLength(255)]
    public string? OwnerName { get; set; } = string.Empty;

    public int? OwnerId { get; set; }

    public int? Type { get; set; }
    public int? Reply { get; set; }
}