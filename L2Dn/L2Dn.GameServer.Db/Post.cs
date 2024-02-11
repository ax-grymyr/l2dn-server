using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace L2Dn.GameServer.Db;

public class Post
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(255)]
    public string? OwnerName { get; set; }
    public int? OwnerId { get; set; }
    
    public DateTime Date { get; set; }
    
    public int TopicId { get; set; }

    [ForeignKey(nameof(TopicId))]
    public Topic Topic { get; set; } = null!;
    
    public int ForumId { get; set; }

    [ForeignKey(nameof(ForumId))]
    public Forum Forum { get; set; } = null!;

    public string Text { get; set; } = string.Empty;
}