using L2Dn.Conversion;
using L2Dn.Cryptography;

namespace L2Dn.AuthServer.Cryptography;

internal sealed class NewAuthPacketEncoder: IPacketEncoder
{
    private readonly ICryptoEngine _staticEngine;
    private readonly ICryptoEngine _engine;
    private bool _staticKey = true;

    /// <summary>
    ///
    /// </summary>
    /// <param name="staticEngine">Static Blowfish parameters for encrypting Init packet.</param>
    /// <param name="engine"></param>
    public NewAuthPacketEncoder(ICryptoEngine staticEngine, ICryptoEngine engine)
    {
        _staticEngine = staticEngine;
        _engine = engine;
    }

    public bool Decode(Span<byte> packet)
    {
        if (!_staticKey)
            return true;

        if ((packet.Length & 7) != 0 || packet.Length < 8)
            return false;

        _engine.Decode(packet, packet);
        return VerifyChecksum(packet);
    }

    public int GetRequiredLength(int length)
    {
        int newLength = length + 4; // reserve checksum

        if (_staticKey)
        {
            newLength += 4; // reserve for XOR key
            newLength += 8 - newLength % 8; // padding
            return newLength;
        }

        newLength += 8 - newLength % 8; // padding
        return newLength;
    }

    public int Encode(Span<byte> buffer, int packetLength)
    {
        int newLength = GetRequiredLength(packetLength);
        Span<byte> packet = buffer[..newLength];
        if (_staticKey)
        {
            EncXorPass(packet);
            _staticEngine.Encode(packet, packet);
            _staticKey = false;
        }
        else
        {
            AppendChecksum(packet);
            _engine.Encode(packet, packet);
        }

        return newLength;
    }

    private static bool VerifyChecksum(Span<byte> data)
    {
        uint checksum = 0;
        int checksumOffset = data.Length - 4;
        for (int i = 0; i < checksumOffset; i += 4)
            checksum ^= LittleEndianBitConverter.ToUInt32(data[i..]);

        uint check = LittleEndianBitConverter.ToUInt32(data[checksumOffset..]);
        return check == checksum;
    }

    private static void AppendChecksum(Span<byte> data)
    {
        uint checksum = 0;
        int checksumOffset = data.Length - 4;
        for (int i = 0; i < checksumOffset; i += 4)
            checksum ^= LittleEndianBitConverter.ToUInt32(data[i..]);

        LittleEndianBitConverter.WriteUInt32(data[checksumOffset..], checksum);
    }

    private static void EncXorPass(Span<byte> data)
    {
        int keyOffset = data.Length - 0x8;
        uint key = RandomGenerator.GetUInt32();
        for (int i = 4; i < keyOffset; i += 4)
        {
            uint temp = LittleEndianBitConverter.ToUInt32(data[i..]);
            key += temp;
            temp ^= key;
            LittleEndianBitConverter.WriteUInt32(data[i..], temp);
        }

        LittleEndianBitConverter.WriteUInt32(data[keyOffset..], key);
    }
}