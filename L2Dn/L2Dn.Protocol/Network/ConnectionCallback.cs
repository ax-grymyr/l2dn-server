namespace L2Dn.Network;

public abstract class ConnectionCallback
{
    private protected ConnectionCallback()
    {
    }
    
    internal abstract void ConnectionClosed(Session session);
}