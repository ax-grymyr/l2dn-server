using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace L2Dn.GameServer.Db;

public class Forum
{
    [Key]
    public int Id { get; set; }

    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    public int? ParentId { get; set; }
    
    [ForeignKey(nameof(ParentId))]
    public Forum? Parent { get; set; }
    
    public int? Post { get; set; } // ???
    public int? Type { get; set; } // ???
    public int? Perm { get; set; } // ???
    public int? OwnerId { get; set; } // ???
}