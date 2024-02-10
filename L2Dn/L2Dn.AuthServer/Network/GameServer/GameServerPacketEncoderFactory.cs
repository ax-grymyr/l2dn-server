using L2Dn.AuthServer.Cryptography;
using L2Dn.Cryptography;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.GameServer;

internal sealed class GameServerPacketEncoderFactory: IPacketEncoderFactory<GameServerSession>
{
    private readonly OldBlowfishEngine _staticBlowfishEngine = new(StaticBlowfishKeys.P447());

    public IPacketEncoder Create(GameServerSession session) => new AuthPacketEncoder(session.BlowfishEngine);
}