namespace L2Dn.Packets;

public class PacketCodeAttribute(byte code, ushort? exCode = null): Attribute
{
    public PacketCodeAttribute(int code): this(ExtractCode(code), ExtractExCode(code))
    {
    }
    
    public byte Code { get; } = code;
    public ushort? ExCode { get; } = exCode;

    private static byte ExtractCode(int code) =>
        code switch
        {
            < 0 or > 0xFFFFFF => throw new ArgumentOutOfRangeException(nameof(code),
                "Packet code must be in the range 0x00 - 0xFFFFFF"),
            
            <= 0xFF => (byte)code,
            
            _ => (byte)(code >> 16)
        };

    private static ushort? ExtractExCode(int code) => code <= 0xFF ? null : (ushort)(code & 0xFFFF);
}