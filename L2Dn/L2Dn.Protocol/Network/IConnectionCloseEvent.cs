namespace L2Dn.Network;

internal interface IConnectionCloseEvent 
{
    void ConnectionClosed(int sessionId);
}
