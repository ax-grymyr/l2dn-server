using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class DbAirShip
{
    [Key]
    public int OwnerId { get; set; }
    
    public int Fuel { get; set; } = 600;
}