using System.Net;

namespace L2Dn.Network;

public abstract class Session
{
    private static int _globalId;

    protected Session()
    {
        Id = Interlocked.Increment(ref _globalId);
    }

    public int Id { get; }

    public Connection? Connection { get; internal set; }
    public IPAddress IpAddress { get; internal set; } = IPAddress.Any;

    protected internal virtual long GetState() => 0;
}