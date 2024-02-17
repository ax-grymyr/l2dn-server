using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class DbMailMessage
{
    [Key]
    public int MessageId { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    
    public string Subject { get; set; } = string.Empty; // TODO: max length
    public string Content { get; set; } = string.Empty; // TODO: max length
    public DateTime ExpirationTime { get; set; }
    public long RequiredAdena { get; set; }
    public bool HasAttachments { get; set; }
    public bool IsUnread { get; set; }
    public bool IsDeletedBySender { get; set; }
    public bool IsDeletedByReceiver { get; set; }
    public bool IsLocked { get; set; }
    public byte SentBySystem { get; set; } // TODO: MailType enum
    public bool IsReturned { get; set; }
    public int ItemId { get; set; }
    public short EnchantLevel { get; set; }
    
    [MaxLength(25)]
    public string? Elementals { get; set; }
}