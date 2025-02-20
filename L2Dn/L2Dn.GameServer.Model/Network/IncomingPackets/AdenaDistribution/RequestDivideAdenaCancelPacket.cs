using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.AdenaDistribution;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.AdenaDistribution;

public struct RequestDivideAdenaCancelPacket: IIncomingPacket<GameSession>
{
    private bool _cancel;

    public void ReadContent(PacketBitReader reader)
    {
        _cancel = reader.ReadByte() == 0;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_cancel)
        {
            AdenaDistributionRequest? request = player.getRequest<AdenaDistributionRequest>();
            if (request != null)
            {
                foreach (Player p in request.getPlayers())
                {
                    if (p != null)
                    {
                        p.sendPacket(SystemMessageId.ADENA_DISTRIBUTION_HAS_BEEN_CANCELLED);
                        p.sendPacket(ExDivideAdenaCancelPacket.STATIC_PACKET);
                        p.removeRequest<AdenaDistributionRequest>();
                    }
                }
            }
        }

        return ValueTask.CompletedTask;
    }
}