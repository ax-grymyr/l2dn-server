using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class DbCastleDoorUpgrade
{
    [Key]
    public int DoorId { get; set; }
    
    public short Ratio { get; set; }
    public byte CastleId { get; set; }
}