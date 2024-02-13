using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(ObjectId), nameof(Name))]
public class BufferScheme
{
    public int ObjectId { get; set; }

    [MaxLength(16)]
    public string Name { get; set; } = "default";

    [MaxLength(200)]
    public string Skills { get; set; } = string.Empty;
}