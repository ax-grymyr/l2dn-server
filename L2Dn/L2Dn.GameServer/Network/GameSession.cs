using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;

namespace L2Dn.GameServer.Network;

public sealed class GameSession(byte[]? encryptionKey): Session
{
    public GameSessionState State { get; set; } = GameSessionState.ProtocolVersion;
    public ServerConfig Config => ServerConfig.Instance;
    public byte[]? EncryptionKey => encryptionKey;
    public int PlayKey1 { get; set; }
    public int AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    //public List<Character> Characters { get; } = new();
    //public Character? SelectedCharacter { get; set; }
    public Player? Player { get; set; }
}