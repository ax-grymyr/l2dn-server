using L2Dn.Packets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Network.Enums;

public class MaskablePacketHelper<T>
    where T: struct, Enum
{
    private readonly int _maskSizeInBytes;
    private ulong _mask;

    public MaskablePacketHelper(int maskSizeInBytes)
    {
        if (maskSizeInBytes is < 1 or > 8)
            throw new ArgumentOutOfRangeException(nameof(maskSizeInBytes));
        
        _maskSizeInBytes = maskSizeInBytes;
    }
    
    public void AddComponent(T component)
    {
        int componentIndex = component.ToInt32();
        if (componentIndex < 0 || componentIndex >= _maskSizeInBytes * 8)
            throw new ArgumentOutOfRangeException(nameof(component), "Invalid component");

        _mask |= GetMask(componentIndex);
    }

    public bool HasComponent(T component)
    {
        int componentIndex = component.ToInt32();
        if (componentIndex < 0 || componentIndex >= _maskSizeInBytes * 8)
            return false;

        return (_mask & GetMask(componentIndex)) != 0;
    }

    private static ulong GetMask(int componentIndex) => 1uL << (componentIndex ^ 7);

    public void WriteMask(PacketBitWriter writer)
    {
        switch (_maskSizeInBytes)
        {
            case 1:
                writer.WriteByte((byte)_mask);
                break;
            case 2:
                writer.WriteUInt16((ushort)_mask);
                break;
            case 3:
                writer.WriteUInt16((ushort)_mask);
                writer.WriteByte((byte)(_mask >> 16));
                break;
            case 4:
                writer.WriteUInt32((uint)_mask);
                break;
            case 5:
                writer.WriteUInt32((uint)_mask);
                writer.WriteByte((byte)(_mask >> 32));
                break;
            case 6:
                writer.WriteUInt32((uint)_mask);
                writer.WriteUInt16((ushort)(_mask >> 32));
                break;
            case 7:
                writer.WriteUInt32((uint)_mask);
                writer.WriteUInt16((ushort)(_mask >> 32));
                writer.WriteByte((byte)(_mask >> 48));
                break;
            case 8:
                writer.WriteUInt64(_mask);
                break;
        }
    }
}