using System.Net.Sockets;
using L2Dn.Cryptography;
using L2Dn.Packets;
using NLog;

namespace L2Dn.Network;

public sealed class Connector<TSession>: IConnectionCloseEvent
    where TSession: ISession
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(Connector<TSession>));
    private readonly string _address;
    private readonly int _port;
    private readonly TSession _session;
    private readonly IPacketEncoder _packetEncoder;
    private readonly IPacketHandler<TSession> _packetHandler;
    private Connection<TSession>? _connection;
    private CancellationToken _cancellationToken;

    public Connector(TSession session, IPacketEncoder packetEncoder, IPacketHandler<TSession> packetHandler,
        string address, int port)
    {
        _address = address;
        _port = port;
        _session = session;
        _packetEncoder = packetEncoder;
        _packetHandler = packetHandler;
    }

    public void Start(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        ConnectAsync(cancellationToken);
    }

    private async void ConnectAsync(CancellationToken cancellationToken)
    {
        try
        {
            TcpClient client = new TcpClient();
            await client.ConnectAsync(_address, _port, cancellationToken).ConfigureAwait(false);
            _logger.Trace($"Connected to {client.Client.RemoteEndPoint}");
            HandleConnection(client, cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            if (exception is SocketException { SocketErrorCode: SocketError.OperationAborted })
            {
                return;
            }
            
            _logger.Error($"Exception in Connector: {exception}");
        }
    } 
    
    private void HandleConnection(TcpClient client, CancellationToken cancellationToken)
    {
        _connection = new Connection<TSession>(this, client, _session, _packetEncoder, _packetHandler);
        _connection.BeginReceivingAsync(cancellationToken);
    }

    void IConnectionCloseEvent.ConnectionClosed(int sessionId)
    {
        if (!_cancellationToken.IsCancellationRequested)
            ConnectAsync(_cancellationToken);
    }
}