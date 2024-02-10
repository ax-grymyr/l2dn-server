namespace L2Dn.AuthServer.Model;

public sealed class AccountInfo
{
    public int AccountId { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public byte? LastServerId { get; set; }
}