using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[Index(nameof(CharacterId))]
public class DbHeroDiary
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int CharacterId { get; set; }
    public DateTime Time { get; set; }
    public byte Action { get; set; }
    public int Param { get; set; }
}