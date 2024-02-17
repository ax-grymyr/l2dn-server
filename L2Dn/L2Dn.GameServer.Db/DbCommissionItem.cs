using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace L2Dn.GameServer.Db;

public class DbCommissionItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int ItemObjectId { get; set; }
    public long PricePerUnit { get; set; }
    public DateTime StartTime { get; set; }
    public short DurationInDays { get; set; }
    public short DiscountInPercentage { get; set; }
}