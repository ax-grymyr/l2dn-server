namespace L2Dn.AuthServer.Network.OutgoingPackets;

internal enum LoginFailReason
{
    SystemError = 1,
    InvalidPassword = 2,
    InvalidLoginOrPassword = 3, // invalid login or password
    AccessDenied = 4,
    InvalidAccount = 5, // invalid account information
    AccountInUse = 7,
    AccountBanned = 9,
    ServerInService = 0x10, // на сервере идут сервисные работы
    AccountEnd = 0x12, // срок действия истек 
    NoTime = 0x13, // на аккаунте не осталось больше времени
}
