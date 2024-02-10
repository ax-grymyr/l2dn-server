namespace L2Dn.AuthServer.Network.Client;

[Flags]
internal enum AuthSessionState
{
    None = 0,
    Authorization = 1,
    GameServerLogin = 2,
}