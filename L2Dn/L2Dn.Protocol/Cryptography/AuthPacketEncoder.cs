using L2Dn.Conversion;

namespace L2Dn.Cryptography
{
    public class AuthPacketEncoder: PacketEncoder
    {
        private readonly ICryptoEngine _cryptoEngine;

        public AuthPacketEncoder(ICryptoEngine cryptoEngine)
        {
            _cryptoEngine = cryptoEngine;
        }

        public override bool Decode(Span<byte> packet)
        {
            if ((packet.Length & 7) != 0 || packet.Length < 8)
                return false;

            _cryptoEngine.Decode(packet, packet);
            return VerifyChecksum(packet);
        }

        public override int GetRequiredLength(int length)
        {
            int newLength = length + 4; // checksum 
            return newLength + 8 - (newLength & 7); // padding
        }

        public override int Encode(Span<byte> buffer, int packetLength)
        {
            int newLength = GetRequiredLength(packetLength); 
            buffer.Slice(packetLength, newLength - packetLength).Clear();
            Span<byte> packet = buffer.Slice(0, newLength);
            CalculateChecksum(packet);
            _cryptoEngine.Encode(packet, packet);
            return newLength;
        }

        private static uint GetChecksum(ReadOnlySpan<byte> packet)
        {
            uint checksum = 0;
            for (int i = 0; i < packet.Length; i += 4)
                checksum ^= LittleEndianBitConverter.ToUInt32(packet.Slice(i, 4));

            return checksum;
        }
        
        private static bool VerifyChecksum(ReadOnlySpan<byte> packet)
        {
            if ((packet.Length & 3) != 0 || packet.Length <= 4)
                return false;

            int checksumOffset = packet.Length - 4;
            uint checksum = GetChecksum(packet.Slice(0, checksumOffset));
            uint check = LittleEndianBitConverter.ToUInt32(packet.Slice(checksumOffset, 4));
            return check == checksum;
        }

        private static void CalculateChecksum(Span<byte> packet)
        {
            int checksumOffset = packet.Length - 4;
            uint checksum = GetChecksum(packet.Slice(0, checksumOffset));
            LittleEndianBitConverter.WriteUInt32(packet.Slice(checksumOffset, 4), checksum);
        }
    }
}