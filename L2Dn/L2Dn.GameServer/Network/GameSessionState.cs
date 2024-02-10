namespace L2Dn.GameServer.Network;

[Flags]
internal enum GameSessionState
{
    ProtocolVersion = 1,
    Authorization = 2,
    CharacterScreen = 4,
    EnteringGame = 8,
    InGame = 16
}
