namespace L2Dn.AuthServer.Network;

[Flags]
internal enum AuthSessionState
{
    None = 0,
    Authorization = 1,
    GameServerLogin = 2,
}