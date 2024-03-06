using System.Collections.Immutable;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Model;
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
    public int ProtocolVersion { get; set; }
    public bool IsProtocolOk { get; set; }
    public ImmutableArray<CharSelectInfoPackage> Characters { get; set; } = ImmutableArray<CharSelectInfoPackage>.Empty;
    public Player? Player { get; set; }
    public object PlayerLock { get; } = new();
    public string? MacAddress { get; set; }
    public bool IsDetached { get; set; }
}