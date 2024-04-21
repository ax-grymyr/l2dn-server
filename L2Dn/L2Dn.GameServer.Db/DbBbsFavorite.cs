using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace L2Dn.GameServer.Db;

public class DbBbsFavorite
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int PlayerId { get; set; }
    
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(127)]
    public string ByPass { get; set; } = string.Empty;
    
    public DateTime Created { get; set; }
}