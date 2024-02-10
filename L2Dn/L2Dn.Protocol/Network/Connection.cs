using System.Buffers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using L2Dn.Conversion;
using L2Dn.Cryptography;
using L2Dn.Packets;

namespace L2Dn.Network;

public sealed class Connection<TSession>
    where TSession: ISession
{
    private record struct PacketData(byte[] Buffer, int Length, SendPacketOptions Options = SendPacketOptions.None);

    private readonly IConnectionCloseEvent _closeEvent;
    private readonly TcpClient _client;
    private readonly TSession _session;
    private readonly IPacketEncoder _packetEncoder;
    private readonly IPacketHandler<TSession> _packetHandler;
    private readonly ConcurrentQueue<PacketData> _sendQueue = new();
    private SpinLock _sendLock; // must not be readonly
    private int _buffersInRent;
    private bool _closed;

    internal Connection(IConnectionCloseEvent closeEvent, TcpClient client, TSession session,
        IPacketEncoder packetEncoder, IPacketHandler<TSession> packetHandler)
    {
        _closeEvent = closeEvent;
        _client = client;
        _session = session;
        _packetEncoder = packetEncoder;
        _packetHandler = packetHandler;

        ConfigureSocket();

        Logger.Trace($"S({_session.Id})  Session connected; remote endpoint {client.Client.RemoteEndPoint}");
        OnConnected();
    }

    public IPAddress? GetRemoteAddress() =>
        _client.Client.RemoteEndPoint is IPEndPoint ipEndPoint ? ipEndPoint.Address : null;

    public TSession Session => _session;
    public bool Closed => _closed;

    public void Send<T>(ref T packet, SendPacketOptions options = SendPacketOptions.None)
        where T: struct, IOutgoingPacket
    {
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
            Logger.Error($"S({_session.Id})  Error serializing packet: {exception}");
            ReturnBuffer(buffer);
            Close();
            return;
        }

        if (offset >= 65536)
        {
            Logger.Warn($"S({_session.Id})  Packet {typeof(T).Name} (0x{buffer[2]:X2}) is too long ({offset} bytes)");
            Close();
            return;
        }

        Logger.Trace($"S({_session.Id})  Sending packet {typeof(T).Name} (0x{buffer[2]:X2}), length: {offset}");
        _sendQueue.Enqueue(new PacketData(buffer, offset, options));
        ThreadPool.QueueUserWorkItem(_ => SendPackets());
    }

    public async void Close()
    {
        if (_closed)
            return;

        try
        {
            _client.Client.Close(100);
        }
        catch (Exception exception)
        {
            Logger.Warn($"S({_session.Id})  Error closing connection: {exception}");
        }
        finally
        {
            _closed = true;
        }

        try
        {
            await _packetHandler.OnDisconnectedAsync(this);
        }
        catch (Exception exception)
        {
            Logger.Warn($"S({_session.Id})  Error in packet handler: {exception}");
        }
        finally
        {
            _closeEvent.ConnectionClosed(_session.Id);
        }

        Logger.Trace($"S({_session.Id})  Session disconnected");
        if (_buffersInRent != 0)
            Logger.Warn($"S({_session.Id})  Rented buffers count = {_buffersInRent}");
    }

    internal async void BeginReceivingAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ReceivePacketAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception exception)
        {
            if (exception is SocketException { SocketErrorCode: SocketError.Success or SocketError.OperationAborted })
            {
                return;
            }

            Logger.Warn($"S({_session.Id})  Error receiving data: {exception}");
            Close();
        }
    }

    private async void OnConnected()
    {
        try
        {
            await _packetHandler.OnConnectedAsync(this);
        }
        catch (Exception exception)
        {
            Logger.Warn($"S({_session.Id})  Exception in packet handler OnConnectedAsync: {exception}");
        }
    }

    private void SendPackets()
    {
        bool lockTaken = false;
        try
        {
            _sendLock.Enter(ref lockTaken);

            // Encrypting and sending packet must be in critical section
            while (_sendQueue.TryDequeue(out var data))
            {
                if (!_closed)
                    SendPacket(data);
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

        try
        {
            if ((options & SendPacketOptions.DontEncrypt) == 0)
            {
                try
                {
                    length = 2 + _packetEncoder.Encode(buffer.AsSpan(2), length - 2);
                }
                catch (Exception exception)
                {
                    Logger.Error($"S({_session.Id})  Error encoding packet: {exception}");
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
                Logger.Error($"S({_session.Id})  Encrypted packet is too long ({length} bytes)");
                Close();
                return;
            }

            // Set packet length
            LittleEndianBitConverter.WriteUInt16(buffer, (ushort)length);

            try
            {
                SendBuffer(buffer.AsSpan(0, length));
            }
            catch (Exception exception)
            {
                Logger.Error($"S({_session.Id})  Error sending packet: {exception}");
                Close();
                return;
            }
        }
        finally
        {
            ReturnBuffer(buffer);
        }

        if ((options & SendPacketOptions.CloseAfterSending) != 0)
            Close();
    }

    private async ValueTask ReceivePacketAsync(CancellationToken cancellationToken)
    {
        byte[] buffer = RentBuffer(1024);

        // Packet length
        await ReceiveAsync(buffer, 2, cancellationToken).ConfigureAwait(false);
        int length = LittleEndianBitConverter.ToUInt16(buffer) - 2;
        if (length > buffer.Length)
        {
            ReturnBuffer(buffer);
            buffer = RentBuffer(length);
        }

        await ReceiveAsync(buffer, length, cancellationToken).ConfigureAwait(false);

        if (!_packetEncoder.Decode(buffer.AsSpan(0, length)))
        {
            Logger.Warn($"S({_session.Id})  Error decoding packet");
            Close();
            return;
        }

        Logger.Trace($"S({_session.Id})  Packet received: code 0x{buffer[0]:X2}, length {length}");
        ThreadPool.QueueUserWorkItem(_ => HandlePacket(buffer, length));
    }

    private async void HandlePacket(byte[] buffer, int length)
    {
        try
        {
            PacketBitReader reader = new(buffer, 0, length);
            await _packetHandler.OnPacketReceivedAsync(this, reader);
        }
        catch (Exception exception)
        {
            Logger.Warn($"S({_session.Id})  Error handling packet: {exception}");
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
                throw new SocketException();

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
                throw new SocketException();

            data = data.Slice(bytesSent);
        }
    }

    private void ConfigureSocket()
    {
        Socket socket = _client.Client;
        socket.LingerState = new LingerOption(true, 30);
        socket.NoDelay = true;
    }

    private byte[] RentBuffer(int size)
    {
        _buffersInRent++;
        return ArrayPool<byte>.Shared.Rent(size);
    }

    private void ReturnBuffer(byte[] buffer)
    {
        _buffersInRent--;
        ArrayPool<byte>.Shared.Return(buffer);
    }
}