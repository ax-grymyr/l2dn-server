namespace L2Dn.Network;

public abstract class Session: ISession
{
    private static int _globalId;

    protected Session()
    {
        Id = Interlocked.Increment(ref _globalId);
    }

    public int Id { get; }
    
    //public Connection? Connection { get; }
}

public abstract class Session<TSessionState>: Session, ISession<TSessionState>
    where TSessionState: struct, Enum
{
    public virtual TSessionState State { get; }
}