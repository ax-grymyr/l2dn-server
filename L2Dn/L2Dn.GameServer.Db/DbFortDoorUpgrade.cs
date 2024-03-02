using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class DbFortDoorUpgrade
{
    [Key]
    public int DoorId { get; set; }
    public byte FortId { get; set; }
    public int Hp { get; set; }
    public int PDef { get; set; }
    public int MDef { get; set; }
}