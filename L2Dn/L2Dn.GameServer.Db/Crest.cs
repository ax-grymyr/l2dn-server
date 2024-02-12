using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class Crest
{
    public int Id { get; set; }
    
    [MaxLength(2176)]
    public byte[] Data { get; set; } = Array.Empty<byte>();
    
    public byte Type { get; set; }
}