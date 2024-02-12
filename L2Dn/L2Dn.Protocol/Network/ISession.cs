namespace L2Dn.Network;

public interface ISession
{
    int Id { get; }
}

public interface ISession<out TSessionState>: ISession
    where TSessionState: struct, Enum
{
    TSessionState State { get; }
}