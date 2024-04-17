using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.AuthServer.Db;

[PrimaryKey(nameof(AccountId), nameof(ServerId))]
public class AccountCharacterData
{
    public int AccountId { get; set; }
    
    [ForeignKey(nameof(AccountId))]
    public Account Account { get; set; } = null!;
    
    public byte ServerId { get; set; }

    [ForeignKey(nameof(ServerId))]
    public GameServer Server { get; set; } = null!;
    
    public byte CharacterCount { get; set; }
}