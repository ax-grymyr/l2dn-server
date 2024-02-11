namespace L2Dn.AuthServer.Network.OutgoingPackets;

internal enum PlayFailReason
{
    SystemError = 0x01,
    WrongUserNameOrPassword = 0x02,
    PasswordDoesNotMatch = 0x03,
    AccessFailed = 0x04,
    TooManyPlayers = 0x0f
}
