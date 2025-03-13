using System.ComponentModel.DataAnnotations;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Db;

public class DbOlympiadNobleEom
{
    [Key]
    public int CharacterId { get; set; }

    public CharacterClass Class { get; set; }
    public int OlympiadPoints { get; set; }
    public short CompetitionsDone { get; set; }
    public short CompetitionsWon { get; set; }
    public short CompetitionsLost { get; set; }
    public short CompetitionsDrawn { get; set; }
}