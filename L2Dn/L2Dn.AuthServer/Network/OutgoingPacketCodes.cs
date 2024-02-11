namespace L2Dn.AuthServer.Network;

public static class OutgoingPacketCodes
{
    public const byte Init = 0x00;
    public const byte LoginFail = 0x01;
    public const byte AccountKicked = 0x02;
    public const byte LoginOk = 0x03;
    public const byte ServerList = 0x04;
    public const byte PlayFail = 0x06;
    public const byte PlayOk = 0x07;
    public const byte GGAuth = 0x0B;
    public const byte LoginOptFail = 0x0D;
    public const byte PIAgreementCheck = 0x11;
    public const byte PIAgreementAck = 0x12;
}