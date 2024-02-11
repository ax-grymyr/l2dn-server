using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(FriendId))]
public class CharacterFriend
{
    public int CharacterId { get; set; }

    [ForeignKey(nameof(CharacterId))]
    public Character Character { get; set; } = null!;
    
    public int FriendId { get; set; }

    [ForeignKey(nameof(FriendId))]
    public Character Friend { get; set; } = null!;
    
    public int Relation { get; set; }
    
    [MaxLength(255)]
    public string? Memo { get; set; }
}