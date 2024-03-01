using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class DbOlympiadData
{
    [Key]
    public byte Id { get; set; }
    
    public short CurrentCycle { get; set; } = 1;
    public short Period { get; set; }
    public DateTime OlympiadEnd { get; set; }
    public DateTime ValidationEnd { get; set; }
    public DateTime NextWeeklyChange { get; set; }
}