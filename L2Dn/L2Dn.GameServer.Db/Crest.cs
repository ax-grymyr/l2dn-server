using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace L2Dn.GameServer.Db;

public class Crest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [MaxLength(2176)]
    public byte[] Data { get; set; } = Array.Empty<byte>();
    
    public byte Type { get; set; }
}