using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestFortressSiegeInfoPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        foreach (Fort fort in FortManager.getInstance().getForts())
        {
            if ((fort != null) && fort.getSiege().isInProgress())
            {
                player.sendPacket(new ExShowFortressSiegeInfoPacket(fort));
            }
        }
        
        return ValueTask.CompletedTask;
    }
}