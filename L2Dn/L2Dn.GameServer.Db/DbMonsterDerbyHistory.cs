using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class DbMonsterDerbyHistory
{
    [Key]
    public int RaceId { get; set; }
    public int First { get; set; }
    public int Second { get; set; }
    public double OddRate { get; set; }
}