namespace L2Dn.Network;

public abstract class Session: ISession
{
    private static int _globalId;
    private readonly int _id;
    
    protected Session()
    {
        _id = Interlocked.Increment(ref _globalId);
    }

    public int Id => _id;
}
