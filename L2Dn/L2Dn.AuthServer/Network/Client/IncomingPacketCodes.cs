namespace L2Dn.AuthServer.Network.Client;

public static class IncomingPacketCodes
{
    public const byte RequestAuthLogin = 0x00;
    public const byte RequestServerLogin = 0x02;
    public const byte RequestServerList = 0x05;
    public const byte RequestGGAuth = 0x07;
    public const byte RequestPIAgreementCheck = 0x0E;
    public const byte RequestPIAgreement = 0x0F;
}