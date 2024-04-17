namespace L2Dn.GameServer.NetworkAuthServer;

public static class OutgoingPacketCodes
{
    public const byte RegisterGameServer = 0x00;
    public const byte UpdateStatus = 0x01;
    public const byte PingRequest = 0x02;
    public const byte AccountStatus = 0x03;
}