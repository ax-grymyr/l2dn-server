using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(ResidenceId), nameof(Id))]
public class DbResidenceFunction
{
    public int Id { get; set; }
    public int Level { get; set; }
    public DateTime Expiration { get; set; }
    public int ResidenceId { get; set; }
}