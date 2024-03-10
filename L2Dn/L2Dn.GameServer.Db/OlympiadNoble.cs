using System.ComponentModel.DataAnnotations;
using L2Dn.Model;

namespace L2Dn.GameServer.Db;

public class OlympiadNoble
{
    [Key]
    public int CharacterId { get; set; }
    
    public CharacterClass Class { get; set; }
    public int OlympiadPoints { get; set; }
    public short CompetitionsDone { get; set; }
    public short CompetitionsWon { get; set; }
    public short CompetitionsLost { get; set; }
    public short CompetitionsDrawn { get; set; }
    public short CompetitionsDoneWeek { get; set; }
}