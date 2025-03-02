using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.AuthServer.Db;

[PrimaryKey(nameof(AccountId), nameof(ServerId))]
public class DbAccountCharacterData
{
    public int AccountId { get; set; }

    [ForeignKey(nameof(AccountId))]
    public DbAccount Account { get; set; } = null!;

    public byte ServerId { get; set; }

    public byte CharacterCount { get; set; }
}