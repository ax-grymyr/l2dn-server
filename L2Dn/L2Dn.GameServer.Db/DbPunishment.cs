using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace L2Dn.GameServer.Db;

public class DbPunishment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(255)]
    public string Key { get; set; } = string.Empty;
 
    public int Affect { get; set; } // TODO enum PunishmentAffect
 
    public int Type { get; set; } // TODO enum PunishmentType

    public DateTime ExpirationTime { get; set; }
    
    public string Reason { get; set; } = string.Empty;
 
    [MaxLength(255)]
    public string PunishedBy { get; set; } = string.Empty;
}