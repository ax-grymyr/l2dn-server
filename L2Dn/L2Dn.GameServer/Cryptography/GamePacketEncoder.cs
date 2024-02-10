using L2Dn.Conversion;
using L2Dn.Cryptography;

namespace L2Dn.GameServer.Cryptography;

internal sealed class GamePacketEncoder: IPacketEncoder
{
    private readonly byte[] _inKey;
    private readonly byte[] _outKey;
    private bool _enabled;

    public GamePacketEncoder(ReadOnlySpan<byte> cryptKey)
    {
        _inKey = cryptKey.ToArray();
        _outKey = cryptKey.ToArray();
    }

    public void Enable()
    {
        _enabled = true;
    }

    public int GetRequiredLength(int length) => length;

    public int Encode(Span<byte> buffer, int packetLength)
    {
        if (!_enabled)
            return packetLength;
        
        byte[] key = _outKey;
        int temp = 0;
        for (int i = 0; i < packetLength; i++)
        {
            temp = buffer[i] ^ key[i & 7] ^ temp;
            buffer[i] = (byte)temp;
        }

        int newValue = NativeEndianBitConverter.ToInt32(key) + packetLength;
        NativeEndianBitConverter.WriteInt32(key, newValue);

        // byte[] key = _encryptionKey;
        // int temp = 0;
        // for (int i = 0; i < packetLength; i++)
        // {
        //     int temp2 = buffer[i];
        //     buffer[i] = (byte)(temp2 ^ key[i & 7] ^ temp);
        //     temp = buffer[i];
        // }
        //
        // int old = key[0];
        // old |= key[1] << 8;
        // old |= key[2] << 0x10;
        // old |= key[3] << 0x18;
		      //
        // old += packetLength;
		      //
        // key[0] = (byte)old;
        // key[1] = (byte)(old >> 0x08);
        // key[2] = (byte)(old >> 0x10);
        // key[3] = (byte)(old >> 0x18);

        return packetLength;
    }

    public bool Decode(Span<byte> packet)
    {
        if (!_enabled)
            return true;

        byte[] key = _inKey;
        int temp = 0;
        for (int i = 0; i < packet.Length; i++)
        {
            int temp2 = packet[i];
            packet[i] = (byte)(temp2 ^ key[i & 7] ^ temp);
            temp = temp2;
        }

        int newValue = NativeEndianBitConverter.ToInt32(key) + packet.Length;
        NativeEndianBitConverter.WriteInt32(key, newValue);
        
        // byte[] key = _inKey;
        // int temp = 0;
        // for (int i = 0; i < packet.Length; i++)
        // {
        //     int temp2 = packet[i];
        //     packet[i] = (byte)(temp2 ^ key[i & 7] ^ temp);
        //     temp = temp2;
        // }
        //
        // int old = key[0];
        // old |= key[1] << 8;
        // old |= key[2] << 0x10;
        // old |= key[3] << 0x18;
		      //
        // old += packet.Length;
		      //
        // key[0] = (byte)old;
        // key[1] = (byte)(old >> 0x08);
        // key[2] = (byte)(old >> 0x10);
        // key[3] = (byte)(old >> 0x18);

        return true;
    }
}
