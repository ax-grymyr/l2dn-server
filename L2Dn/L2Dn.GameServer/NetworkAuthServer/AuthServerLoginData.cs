namespace L2Dn.GameServer.NetworkAuthServer;

public record AuthServerLoginData(
    int AccountId,
    string AccountName,
    DateTime TimeStamp,
    int LoginKey1,
    int LoginKey2,
    int PlayKey1,
    int PlayKey2);