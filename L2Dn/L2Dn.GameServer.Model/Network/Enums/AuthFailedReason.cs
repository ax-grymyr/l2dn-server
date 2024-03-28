namespace L2Dn.GameServer.Network.OutgoingPackets;

internal enum AuthFailedReason
{
    NoText = 0,
    SystemErrorLoginLater = 1,
    PasswordDoesNotMatchThisAccount = 2,
    PasswordDoesNotMatchThisAccount2 = 3,
    AccessFailedTryLater = 4,
    IncorrectAccountInfoContactCustomerSupport = 5,
    AccessFailedTryLater2 = 6,
    AccountAlreadyInUse = 7,
    AccessFailedTryLater3 = 8,
    AccessFailedTryLater4 = 9,
    AccessFailedTryLater5 = 10,
}