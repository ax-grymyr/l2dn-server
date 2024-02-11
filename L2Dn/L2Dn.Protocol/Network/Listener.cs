using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using L2Dn.Extensions;
using L2Dn.Packets;
using NLog;

namespace L2Dn.Network;

public sealed class Listener<TSession>: IConnectionCloseEvent
    where TSession: ISession
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(Listener<TSession>));
    private readonly ConcurrentDictionary<int, Connection<TSession>> _connections = new();
    private readonly ISessionFactory<TSession> _sessionFactory;
    private readonly IPacketEncoderFactory<TSession> _packetEncoderFactory;
    private readonly IPacketHandler<TSession> _packetHandler;
    private readonly TcpListener _listener;

    public Listener(ISessionFactory<TSession> sessionFactory,
        IPacketEncoderFactory<TSession> packetEncoderFactory,
        IPacketHandler<TSession> packetHandler,
        IPAddress address, int port)
    {
        _sessionFactory = sessionFactory;
        _packetEncoderFactory = packetEncoderFactory;
        _packetHandler = packetHandler;
        
        _listener = new TcpListener(address, port);
        if (address.AddressFamily == AddressFamily.InterNetworkV6)
            _listener.Server.DualMode = true;
    }

    public Task Start(CancellationToken cancellationToken)
    {
        _listener.Start();
        return AcceptConnectionsAsync(cancellationToken);
    }

    private async Task AcceptConnectionsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                TcpClient client = await _listener.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
                _logger.Trace($"Accepted connection from {client.Client.RemoteEndPoint}");
                ThreadPool.QueueUserWorkItem(_ => { HandleConnection(client); });
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                if (exception is SocketException { SocketErrorCode: SocketError.OperationAborted })
                {
                    break;
                }
                
                _logger.Error($"Exception in Listener: {exception}");
            }
        }

        try
        {
            _listener.Stop();
            _connections.Values.ForEach(c => c.Close());
        }
        catch (Exception exception)
        {
            _logger.Error($"Exception in Listener: {exception}");
        }
    }
    
    private void HandleConnection(TcpClient client)
    {
        TSession session = _sessionFactory.Create();
        Connection<TSession> connection = _connections.GetOrAdd(session.Id,
            id => new Connection<TSession>(this, client, session, _packetEncoderFactory.Create(session), _packetHandler));

        if (!ReferenceEquals(session, connection.Session))
        {
            _logger.Error($"Duplicated session id: {session.Id}");
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