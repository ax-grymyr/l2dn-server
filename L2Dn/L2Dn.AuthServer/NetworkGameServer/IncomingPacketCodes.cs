namespace L2Dn.AuthServer.NetworkGameServer;

public static class IncomingPacketCodes
{
    public const byte RegisterGameServer = 0x00;
    public const byte UpdateStatus = 0x01;
    public const byte PingRequest = 0x02;
    public const byte AccountStatus = 0x03;
    public const byte ChangePassword = 0x04;
}