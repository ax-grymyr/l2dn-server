using System.Net.Sockets;
using L2Dn.Cryptography;
using L2Dn.Packets;
using NLog;

namespace L2Dn.Network;

public sealed class Connector<TSession>(
    TSession session, PacketEncoder packetEncoder, PacketHandler<TSession> packetHandler,
    string address, int port)
    : ConnectionCallback
    where TSession: Session
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(Connector<TSession>));
    private Connection<TSession>? _connection;
    private CancellationToken _cancellationToken;

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
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await client.ConnectAsync(address, port, cancellationToken).ConfigureAwait(false);
                    break;
                }
                catch (SocketException socketException)
                {
                    if (socketException.SocketErrorCode != SocketError.ConnectionRefused)
                        throw;
                }

                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            }

            _logger.Trace($"Connected to {client.Client.RemoteEndPoint}");
            _connection = new Connection<TSession>(this, client, session, packetEncoder, packetHandler);
            _connection.BeginReceivingAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            if (exception is SocketException { SocketErrorCode: SocketError.OperationAborted })
                return;

            _logger.Error($"Exception in Connector: {exception}");
        }
    }

    internal override void ConnectionClosed(Session session)
    {
        if (!_cancellationToken.IsCancellationRequested)
            ConnectAsync(_cancellationToken);
    }
}