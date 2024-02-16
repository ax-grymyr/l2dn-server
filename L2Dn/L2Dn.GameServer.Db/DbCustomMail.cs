namespace L2Dn.GameServer.Db;

public class DbCustomMail
{
    public DateTime Time { get; set; }
    public int ReceiverId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Items { get; set; } = string.Empty;
}