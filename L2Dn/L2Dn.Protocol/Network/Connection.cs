using System.Buffers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using L2Dn.Conversion;
using L2Dn.Cryptography;
using L2Dn.Packets;
using NLog;

namespace L2Dn.Network;

public abstract class Connection
{
    private record struct PacketData(byte[] Buffer, int Length, SendPacketOptions Options = SendPacketOptions.None);

    private static readonly Logger _logger = LogManager.GetLogger(nameof(Connection)); 
    private readonly ConnectionCallback _callback;
    private readonly TcpClient _client;
    private readonly Session _session;
    private readonly PacketEncoder _packetEncoder;
    private readonly ConcurrentQueue<PacketData> _sendQueue = new();
    private SpinLock _sendLock; // must not be readonly
    private int _buffersInRent;
    private bool _closed;

    private protected Connection(ConnectionCallback callback, TcpClient client, Session session,
        PacketEncoder packetEncoder)
    {
        _callback = callback;
        _client = client;
        _session = session;
        _packetEncoder = packetEncoder;

        ConfigureSocket();
    }

    public IPAddress? GetRemoteAddress() =>
        _client.Client.RemoteEndPoint is IPEndPoint ipEndPoint ? ipEndPoint.Address : null;
    
    public bool Closed => _closed;

    public void Send<T>(T packet, SendPacketOptions options = SendPacketOptions.None)
        where T: struct, IOutgoingPacket
    {
        if (_closed)
            return;
        
        Send(ref packet, options);
    }

    public void Send<T>(ref T packet, SendPacketOptions options = SendPacketOptions.None)
        where T: struct, IOutgoingPacket
    {
        if (_closed)
            return;
        
        // Serialize the packet
        byte[] buffer = RentBuffer(65536);
        int offset = 0;
        PacketBitWriter writer = new(buffer, ref offset);
        try
        {
            writer.Skip(2); // reserve 2 bytes for packet length
            packet.WriteContent(writer);
        }
        catch (Exception exception)
        {
            _logger.Error($"S({_session.Id})  Error serializing packet: {exception}");
            Close();
            return;
        }

        if (offset >= 65536)
        {
            _logger.Warn($"S({_session.Id})  Packet {typeof(T).Name} ({buffer[2]:X2}) is too long ({offset} bytes)");
            Close();
            return;
        }

        _logger.Trace($"S({_session.Id})  Sending packet {typeof(T).Name} ({buffer[2]:X2}), length: {offset}");
        _sendQueue.Enqueue(new PacketData(buffer, offset, options));
        ThreadPool.QueueUserWorkItem(_ => SendPackets());
    }

    public void Close()
    {
        if (_closed)
            return;

        try
        {
            _client.Client.Close(1000);
        }
        catch (Exception exception)
        {
            _logger.Warn($"S({_session.Id})  Error closing connection: {exception}");
        }
        finally
        {
            _closed = true;
        }

        try
        {
            OnDisconnected();
        }
        catch (Exception exception)
        {
            _logger.Warn($"S({_session.Id})  Exception in packet handler: {exception}");
        }
        finally
        {
            _callback.ConnectionClosed(_session);
        }

        // return buffers in queue
        while (_sendQueue.TryDequeue(out PacketData packetData))
            ReturnBuffer(packetData.Buffer);
        
        _logger.Trace($"S({_session.Id})  Session disconnected");
        
        if (_buffersInRent > 2)
            _logger.Warn($"S({_session.Id})  Rented buffers count = {_buffersInRent}");
    }

    protected abstract void OnDisconnected();
    protected abstract ValueTask OnPacketReceivedAsync(PacketBitReader reader);
    
    internal async void BeginReceivingAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ReceivePacketAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            Close();
        }
        catch (SocketException socketException)
        {
            SocketError errorCode = socketException.SocketErrorCode;
            if (errorCode == SocketError.Success)
                return;
            
            if (errorCode != SocketError.OperationAborted && errorCode != SocketError.ConnectionReset)
                _logger.Warn($"S({_session.Id})  Error receiving data: {socketException}");
            
            Close();
        }
        catch (Exception exception)
        {
            _logger.Warn($"S({_session.Id})  Error receiving data: {exception}");
            Close();
        }
    }

    private protected static Logger Logger => _logger;
    
    private void SendPackets()
    {
        bool lockTaken = false;
        try
        {
            _sendLock.Enter(ref lockTaken);
            if (!lockTaken)
                throw new SynchronizationLockException("Could not enter send lock");

            // Encrypting and sending packet must be in critical section
            while (_sendQueue.TryDequeue(out PacketData data))
            {
                try
                {
                    if (!_closed)
                        SendPacket(data);
                }
                finally
                {
                    ReturnBuffer(data.Buffer);
                }
            }
        }
        finally
        {
            if (lockTaken)
                _sendLock.Exit();
        }
    }

    private void SendPacket(in PacketData data)
    {
        byte[] buffer = data.Buffer;
        int length = data.Length;
        SendPacketOptions options = data.Options;

        if ((options & SendPacketOptions.DontEncrypt) == 0)
        {
            try
            {
                length = 2 + _packetEncoder.Encode(buffer.AsSpan(2), length - 2);
            }
            catch (Exception exception)
            {
                _logger.Error($"S({_session.Id})  Error encoding packet: {exception}");
                Close();
                return;
            }
        }
        else if ((options & SendPacketOptions.NoPadding) == 0)
        {
            int newLength = 2 + _packetEncoder.GetRequiredLength(length - 2);
            buffer.AsSpan(length, newLength - length).Clear();
            length = newLength;
        }

        if (length >= 65536)
        {
            _logger.Error($"S({_session.Id})  Encrypted packet is too long ({length} bytes)");
            Close();
            return;
        }

        // Set packet length
        LittleEndianBitConverter.WriteUInt16(buffer, (ushort)length);

        try
        {
            SendBuffer(buffer.AsSpan(0, length));
        }
        catch (SocketException socketException)
        {
            SocketError errorCode = socketException.SocketErrorCode;
            if (errorCode == SocketError.Success)
                return;
            
            if (errorCode != SocketError.OperationAborted && errorCode != SocketError.ConnectionReset)
                _logger.Warn($"S({_session.Id})  Error sending packet: {socketException}");
            
            Close();
            return;
        }
        catch (Exception exception)
        {
            _logger.Error($"S({_session.Id})  Error sending packet: {exception}");
            Close();
            return;
        }

        if ((options & SendPacketOptions.CloseAfterSending) != 0)
            Close();
    }

    private async ValueTask ReceivePacketAsync(CancellationToken cancellationToken)
    {
        byte[] buffer = RentBuffer(4096);

        // Packet length
        try
        {
            await ReceiveAsync(buffer, 2, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            ReturnBuffer(buffer);
            throw;
        }

        int length = LittleEndianBitConverter.ToUInt16(buffer) - 2;
        if (length > buffer.Length)
        {
            ReturnBuffer(buffer);
            buffer = RentBuffer(length);
        }

        try
        {
            await ReceiveAsync(buffer, length, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            ReturnBuffer(buffer);
            throw;
        }

        if (!_packetEncoder.Decode(buffer.AsSpan(0, length)))
        {
            _logger.Warn($"S({_session.Id})  Error decoding packet");
            ReturnBuffer(buffer);
            Close();
            return;
        }

        ThreadPool.QueueUserWorkItem(_ => HandlePacket(buffer, length));
    }

    private async void HandlePacket(byte[] buffer, int length)
    {
        try
        {
            PacketBitReader reader = new(buffer, 0, length);
            await OnPacketReceivedAsync(reader);
        }
        catch (Exception exception)
        {
            _logger.Warn($"S({_session.Id})  Error handling packet: {exception}");
        }
        finally
        {
            ReturnBuffer(buffer);
        }
    }

    private async ValueTask ReceiveAsync(byte[] buffer, int count, CancellationToken cancellationToken)
    {
        int offset = 0;
        while (count > 0)
        {
            int bytesReceived = await _client.Client.ReceiveAsync(buffer.AsMemory(offset, count), cancellationToken)
                .ConfigureAwait(false);

            if (bytesReceived == 0)
                throw new SocketException((int)SocketError.OperationAborted, "Error receiving data");

            offset += bytesReceived;
            count -= bytesReceived;
        }
    }

    private void SendBuffer(ReadOnlySpan<byte> buffer)
    {
        // Console.WriteLine($"Sending packet data, length: {buffer.Length}");
        // LogUtils.TracePacketData(buffer, _session.Id);

        ReadOnlySpan<byte> data = buffer;
        while (data.Length > 0)
        {
            int bytesSent = _client.Client.Send(data, SocketFlags.None);
            if (bytesSent == 0)
                throw new SocketException((int)SocketError.OperationAborted, "Error sending data");

            data = data[bytesSent..];
        }
    }

    private void ConfigureSocket()
    {
        Socket socket = _client.Client;
        socket.LingerState = new LingerOption(true, 10);
        socket.NoDelay = true;
    }

    private byte[] RentBuffer(int size)
    {
        Interlocked.Increment(ref _buffersInRent);
        return ArrayPool<byte>.Shared.Rent(size);
    }

    private void ReturnBuffer(byte[] buffer)
    {
        ArrayPool<byte>.Shared.Return(buffer);
        Interlocked.Decrement(ref _buffersInRent);
    }
}

internal sealed class Connection<TSession>: Connection
    where TSession: Session
{
    private readonly TSession _session;
    private readonly PacketHandler<TSession> _packetHandler;

    internal Connection(ConnectionCallback callback, TcpClient client, TSession session,
        PacketEncoder packetEncoder, PacketHandler<TSession> packetHandler)
        : base(callback, client, session, packetEncoder)
    {
        session.Connection = this;
        
        if (client.Client.RemoteEndPoint is IPEndPoint ipEndPoint)
            session.IpAddress = ipEndPoint.Address;
        
        _session = session;
        _packetHandler = packetHandler;

        try
        {
            _packetHandler.OnConnectedInternal(this, _session);
        }
        catch (Exception exception)
        {
            Logger.Warn($"S({_session.Id})  Exception in packet handler: {exception}");
        }

        Logger.Trace($"S({_session.Id})  Session connected; remote endpoint {client.Client.RemoteEndPoint}");
    }

    protected override void OnDisconnected() => _packetHandler.OnDisconnectedInternal(this, _session);

    protected override ValueTask OnPacketReceivedAsync(PacketBitReader reader) =>
        _packetHandler.OnPacketReceivedAsync(this, _session, reader);
}