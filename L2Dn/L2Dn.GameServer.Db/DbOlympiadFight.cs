using System.ComponentModel.DataAnnotations.Schema;
using L2Dn.GameServer.Enums;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[Index(nameof(Character1Id))]
[Index(nameof(Character2Id))]
public class DbOlympiadFight
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int Character1Id { get; set; }
    public int Character2Id { get; set; }

    public CharacterClass Character1Class { get; set; }
    public CharacterClass Character2Class { get; set; }
    public byte Winner { get; set; }
    public DateTime Start { get; set; }
    public TimeSpan Time { get; set; }
    public bool Classed { get; set; }
}