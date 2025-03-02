using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class DbMonsterDerbyBet
{
    [Key]
    public byte LaneId { get; set; }

    public long Bet { get; set; }
}