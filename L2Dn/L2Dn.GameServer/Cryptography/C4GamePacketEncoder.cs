﻿using L2Dn.Cryptography;

namespace L2Dn.GameServer.Cryptography;

internal sealed class C4GamePacketEncoder: PacketEncoder
{
    private readonly byte[] _inKey;
    private readonly byte[] _outKey;
    private bool _enabled;

    public C4GamePacketEncoder(ReadOnlySpan<byte> cryptKey)
    {
        _inKey = cryptKey.ToArray();
        _outKey = cryptKey.ToArray();
    }

    public void Enable()
    {
        _enabled = true;
    }

    public override int GetRequiredLength(int length) => length;

    public override int Encode(Span<byte> buffer, int packetLength)
    {
        if (!_enabled)
        {
            _enabled = true;
            return packetLength;
        }
        
        // byte[] key = _outKey;
        // int temp = 0;
        // for (int i = 0; i < packetLength; i++)
        // {
        //     temp = buffer[i] ^ key[i & 7] ^ temp;
        //     buffer[i] = (byte)temp;
        // }
        //
        // int newValue = LittleEndianBitConverter.ToInt32(key) + packetLength;
        // LittleEndianBitConverter.WriteInt32(key, newValue);

        byte[] key = _outKey;
        int temp = 0;
        for (int i = 0; i < packetLength; i++)
        {
            int temp2 = buffer[i];
            temp = temp2 ^ key[i & 7] ^ temp;
            buffer[i] = (byte)temp;
        }
        
        int old = key[0];
        old |= key[1] << 8;
        old |= key[2] << 0x10;
        old |= key[3] << 0x18;
		      
        old += packetLength;
		      
        key[0] = (byte)old;
        key[1] = (byte)(old >> 0x08);
        key[2] = (byte)(old >> 0x10);
        key[3] = (byte)(old >> 0x18);

        return packetLength;
    }

    public override bool Decode(Span<byte> packet)
    {
        if (!_enabled)
            return true;

        // byte[] key = _inKey;
        // int temp = 0;
        // for (int i = 0; i < packet.Length; i++)
        // {
        //     int temp2 = packet[i];
        //     packet[i] = (byte)(temp2 ^ key[i & 7] ^ temp);
        //     temp = temp2;
        // }
        //
        // int newValue = LittleEndianBitConverter.ToInt32(key) + packet.Length;
        // LittleEndianBitConverter.WriteInt32(key, newValue);
        
        byte[] key = _inKey;
        int temp = 0;
        for (int i = 0; i < packet.Length; i++)
        {
            int temp2 = packet[i];
            packet[i] = (byte)(temp2 ^ key[i & 7] ^ temp);
            temp = temp2;
        }
        
        int old = key[0];
        old |= key[1] << 8;
        old |= key[2] << 0x10;
        old |= key[3] << 0x18;
		      
        old += packet.Length;
		      
        key[0] = (byte)old;
        key[1] = (byte)(old >> 0x08);
        key[2] = (byte)(old >> 0x10);
        key[3] = (byte)(old >> 0x18);

        return true;
    }
}