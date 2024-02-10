using L2Dn.Cryptography;

namespace L2Dn.Packets;

public interface IPacketEncoderFactory<in TSession>
{
    IPacketEncoder Create(TSession session);
}
