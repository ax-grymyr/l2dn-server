using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(ItemId), nameof(Name))]
public class DbItemVariable
{
    public int ItemId { get; set; }

    [MaxLength(255)]
    public string Name { get; set; }

    public string Value { get; set; }
}