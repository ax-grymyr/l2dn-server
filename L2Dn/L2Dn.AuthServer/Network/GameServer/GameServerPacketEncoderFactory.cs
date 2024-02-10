using L2Dn.AuthServer.Cryptography;
using L2Dn.Cryptography;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.GameServer;

internal sealed class GameServerPacketEncoderFactory: IPacketEncoderFactory<GameServerSession>
{
    private readonly BlowfishEngine _staticBlowfishEngine = new(StaticBlowfishKeys.GameServer());

    public IPacketEncoder Create(GameServerSession session) => new AuthPacketEncoder(_staticBlowfishEngine);
}