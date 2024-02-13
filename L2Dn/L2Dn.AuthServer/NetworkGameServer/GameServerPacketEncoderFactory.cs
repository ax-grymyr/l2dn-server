using L2Dn.AuthServer.Cryptography;
using L2Dn.Cryptography;
using L2Dn.Packets;

namespace L2Dn.AuthServer.NetworkGameServer;

internal sealed class GameServerPacketEncoderFactory: IPacketEncoderFactory<GameServerSession>
{
    private readonly BlowfishEngine _staticBlowfishEngine = new(StaticBlowfishKeys.GameServer());

    public PacketEncoder Create(GameServerSession session) => new AuthPacketEncoder(_staticBlowfishEngine);
}