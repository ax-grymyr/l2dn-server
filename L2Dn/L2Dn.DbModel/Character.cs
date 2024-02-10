using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.DbModel;

[Table(nameof(Character) + "s")]
[Index(nameof(ServerId), nameof(Name), IsUnique = true)]
public class Character: VersionedEntity
{
    public int AccountId { get; set; }
    
    [ForeignKey(nameof(AccountId))]
    public Account? Account { get; set; }
    
    public int ServerId { get; set; }
    
    [ForeignKey(nameof(ServerId))]
    public GameServer? Server { get; set; }

    [Required]
    [MaxLength(35)]
    public string Name { get; set; } = string.Empty;

    public int NameColor { get; set; } = 0xFFFFFF;

    [Required]
    [MaxLength(35)]
    public string Title { get; set; } = string.Empty;

    public int TitleColor { get; set; } = 0xECF9A2;

    public Sex Sex { get; set; }
    public byte HairStyle { get; set; }
    public byte HairColor { get; set; }
    public byte Face { get; set; }
    public CharacterClass Class { get; set; }

    public long Exp { get; set; }
    public long ExpBeforeDeath { get; set; }
    public long Sp { get; set; }
    public int Reputation { get; set; }
    public int PkCounter { get; set; }
    public int PvpCounter { get; set; }
    
    public int MaxHp { get; set; }
    public int MaxMp { get; set; }
    public int MaxCp { get; set; }
    public int CurrentHp { get; set; }
    public int CurrentMp { get; set; }
    public int CurrentCp { get; set; }
    
    public int LocationX { get; set; }
    public int LocationY { get; set; }
    public int LocationZ { get; set; }
    public int Heading { get; set; }

    public DateTime Created { get; set; }
    public DateTime? LastAccess { get; set; }
    public DateTime? DeleteTime { get; set; }
}
