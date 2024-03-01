using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[Index(nameof(Name), IsUnique = true)]
[Index(nameof(LeaderId), IsUnique = true)]
[Index(nameof(AllyId))]
public class Clan
{
    public int Id { get; set; }

    [MaxLength(70)]
    public string Name { get; set; } = string.Empty;
    
    public byte Level { get; set; }
    
    public int Reputation { get; set; }
    
    public short? Castle { get; set; } // TODO: must be Castle.OwnerClanId property 

    public short BloodAllianceCount { get; set; }
    public short BloodOathCount { get; set; }
    
    public int? AllyId { get; set; }
    
    [MaxLength(70)]
    public string? AllyName { get; set; }
    public int? AllyCrestId { get; set; }
    
    // [ForeignKey(nameof(AllyId))]
    // public Ally? Ally { get; set; }
    
    public int LeaderId { get; set; }

    [ForeignKey(nameof(LeaderId))]
    public Character Leader { get; set; } = null!;
    
    public int? CrestId { get; set; }
    
    [ForeignKey(nameof(CrestId))]
    public Crest? Crest { get; set; }
    
    public int? LargeCrestId { get; set; }
    
    [ForeignKey(nameof(LargeCrestId))]
    public Crest? LargeCrest { get; set; }
 
    public int AuctionBidAt { get; set; }
    
    public DateTime? AllyPenaltyExpireTime { get; set; }
    public byte AllyPenaltyExpireType { get; set; }
    public DateTime? CharPenaltyExpireTime { get; set; }
    public DateTime? DissolvingExpireTime { get; set; }
    
    public int? NewLeaderId { get; set; }
    public int Exp { get; set; }
}