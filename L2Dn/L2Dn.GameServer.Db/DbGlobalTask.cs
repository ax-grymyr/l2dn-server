using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace L2Dn.GameServer.Db;

public class DbGlobalTask
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(50)]
    public string TaskName { get; set; } = string.Empty;

    public int TaskType { get; set; }

    public DateTime? LastRun { get; set; }

    [MaxLength(100)]
    public string? TaskParam1 { get; set; }

    [MaxLength(100)]
    public string? TaskParam2 { get; set; }

    [MaxLength(100)]
    public string? TaskParam3 { get; set; }
}