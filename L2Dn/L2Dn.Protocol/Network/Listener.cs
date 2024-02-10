using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using L2Dn.Logging;
using L2Dn.Packets;

namespace L2Dn.Network;

public sealed class Listener<TSession>: IConnectionCloseEvent
    where TSession: ISession
{
    private readonly ConcurrentDictionary<int, Connection<TSession>> _connections = new();
    private readonly ISessionFactory<TSession> _sessionFactory;
    private readonly IPacketEncoderFactory<TSession> _packetEncoderFactory;
    private readonly IPacketHandler<TSession> _packetHandler;
    private readonly BufferPool _bufferPool;
    private readonly TcpListener _listener;
    private bool _stopRequested;

    public Listener(ISessionFactory<TSession> sessionFactory,
        IPacketEncoderFactory<TSession> packetEncoderFactory,
        IPacketHandler<TSession> packetHandler,
        BufferPool bufferPool,
        IPAddress address, int port)
    {
        _sessionFactory = sessionFactory;
        _packetEncoderFactory = packetEncoderFactory;
        _packetHandler = packetHandler;
        _bufferPool = bufferPool;
        
        _listener = new(address, port);
        if (address.AddressFamily == AddressFamily.InterNetworkV6)
            _listener.Server.DualMode = true;
    }

    public async Task Start()
    {
        _stopRequested = false;
        _listener.Start();
        while (!_stopRequested)
        {
            try
            {
                TcpClient client = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                Logger.Trace($"Accepted connection from {client.Client.RemoteEndPoint}");
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    HandleConnection(client);
                });
            }
            catch (Exception exception)
            {
                if (exception is SocketException { SocketErrorCode: SocketError.OperationAborted })
                {
                    return;
                }
                
                Logger.Error($"Exception in Listener: {exception}");
            }
        }
    }

    public void Stop()
    {
        _stopRequested = true;
        _listener.Stop();
    }

    private void HandleConnection(TcpClient client)
    {
        TSession session = _sessionFactory.Create();
        Connection<TSession> connection = _connections.GetOrAdd(session.Id,
            id => new Connection<TSession>(this, client, session, _packetEncoderFactory.Create(session), _packetHandler,
                _bufferPool));

        if (!ReferenceEquals(session, connection.Session))
        {
            Logger.Error($"Duplicated session id: {session.Id}");
            connection.Close();
            return;
        }
        
        connection.BeginReceivingAsync(default);
    }

    void IConnectionCloseEvent.ConnectionClosed(int sessionId)
    {
        _connections.TryRemove(sessionId, out _);
    }
}
