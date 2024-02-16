using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class PledgeWaitingList
{
    [Key]
    public int CharacterId { get; set; }
    public int Karma { get; set; }
}