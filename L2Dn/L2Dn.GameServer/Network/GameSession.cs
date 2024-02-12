using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;

namespace L2Dn.GameServer.Network;

internal sealed class GameSession(byte[]? encryptionKey): Session, ISession<GameSessionState>
{
    public GameSessionState State { get; set; } = GameSessionState.ProtocolVersion;
    public Connection<GameSession>? Connection { get; set; }
    public ServerConfig Config => ServerConfig.Instance;
    public byte[]? EncryptionKey => encryptionKey;
    public int PlayKey1 { get; set; }
    public int AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public List<Character> Characters { get; } = new();
    public Character? SelectedCharacter { get; set; }
    public int ObjectId { get; set; } = 0x10000548;
    public Location Location { get; set; }
    public Player? Player { get; set; }
}
