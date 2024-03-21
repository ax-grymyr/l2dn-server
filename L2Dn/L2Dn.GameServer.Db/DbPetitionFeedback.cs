using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace L2Dn.GameServer.Db;

public class DbPetitionFeedback
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(35)]
    public string CharacterName { get; set; } = string.Empty;

    [MaxLength(35)]
    public string GmName { get; set; } = string.Empty;

    public byte Rate { get; set; }

    [MaxLength(4096)]
    public string? Message { get; set; }

    public DateTime Time { get; set; }
}