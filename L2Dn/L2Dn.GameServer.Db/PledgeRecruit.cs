using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class PledgeRecruit
{
    [Key]
    public int ClanId { get; set; }
    public int Karma { get; set; }

    [MaxLength(50)]
    public string Information { get; set; } = string.Empty;

    [MaxLength(255)]
    public string DetailedInformation { get; set; } = string.Empty;
    
    public int ApplicationType { get; set; }
    public int RecruitType { get; set; }
}