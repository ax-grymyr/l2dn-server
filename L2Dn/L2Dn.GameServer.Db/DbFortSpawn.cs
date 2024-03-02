using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[Index(nameof(FortId))]
public class DbFortSpawn
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public byte FortId { get; set; }
    public int NpcId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public int Heading { get; set; }
    public byte Type { get; set; } // 0-always spawned, 1-despawned during siege, 2-despawned 10min before siege, 3-spawned after fort taken
    public byte CastleId { get; set; } // Castle ID for Special Envoys
}