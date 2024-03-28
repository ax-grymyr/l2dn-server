using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct LoginFailPacket: IOutgoingPacket
{
    public const int NO_TEXT = 0;
    public const int SYSTEM_ERROR_LOGIN_LATER = 1;
    public const int PASSWORD_DOES_NOT_MATCH_THIS_ACCOUNT = 2;
    public const int PASSWORD_DOES_NOT_MATCH_THIS_ACCOUNT2 = 3;
    public const int ACCESS_FAILED_TRY_LATER = 4;
    public const int INCORRECT_ACCOUNT_INFO_CONTACT_CUSTOMER_SUPPORT = 5;
    public const int ACCESS_FAILED_TRY_LATER2 = 6;
    public const int ACOUNT_ALREADY_IN_USE = 7;
    public const int ACCESS_FAILED_TRY_LATER3 = 8;
    public const int ACCESS_FAILED_TRY_LATER4 = 9;
    public const int ACCESS_FAILED_TRY_LATER5 = 10;
	
    public static readonly LoginFailPacket LOGIN_SUCCESS = new(-1, NO_TEXT);
	
    private readonly int _reason;
    private readonly int _success;
	
    public LoginFailPacket(int reason)
    {
        _success = 0;
        _reason = reason;
    }
	
    public LoginFailPacket(int success, int reason)
    {
        _success = success;
        _reason = reason;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.LOGIN_FAIL);
        
        writer.WriteInt32(_success);
        writer.WriteInt32(_reason);
    }
}