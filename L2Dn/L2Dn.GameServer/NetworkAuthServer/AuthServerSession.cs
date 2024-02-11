using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Model;
using L2Dn.Network;

namespace L2Dn.GameServer.NetworkAuthServer;

internal sealed class AuthServerSession(byte[]? encryptionKey): Session, ISession<AuthServerSessionState>
{
    public AuthServerSessionState State => AuthServerSessionState.None; // not used
    public ServerConfig Config => ServerConfig.Instance;
    public byte[]? EncryptionKey => encryptionKey;
    public int PlayKey1 { get; set; }
    public int AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public int ObjectId { get; set; } = 0x10000548;
    public Location Location { get; set; }
}
