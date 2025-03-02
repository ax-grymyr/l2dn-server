using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class DbAccountPremium
{
    [Key]
    public int AccountId { get; set; }

    public DateTime EndTime { get; set; }
}