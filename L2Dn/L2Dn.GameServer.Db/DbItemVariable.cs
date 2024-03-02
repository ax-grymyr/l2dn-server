using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(ItemId), nameof(Name))]
public class DbItemVariable: DbVariable
{
    public int ItemId { get; set; }
}