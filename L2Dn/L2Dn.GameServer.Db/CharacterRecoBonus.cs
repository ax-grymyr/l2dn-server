using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class CharacterRecoBonus
{
    [Key]
    public int CharacterId { get; set; }
    
    public short RecHave { get; set; }
    public short RecLeft { get; set; }
    public TimeSpan TimeLeft { get; set; }
}