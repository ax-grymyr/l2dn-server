namespace L2Dn.GameServer.Network;

[Flags]
public enum GameSessionState: uint
{
    ProtocolVersion = 1,
    Authorization = 2,
    CharacterScreen = 4,
    EnteringGame = 8,
    InGame = 16,
    
    All = uint.MaxValue
}