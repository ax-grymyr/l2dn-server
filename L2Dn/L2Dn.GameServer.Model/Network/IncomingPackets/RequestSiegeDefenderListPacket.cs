using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestSiegeDefenderListPacket: IIncomingPacket<GameSession>
{
    private int _castleId;

    public void ReadContent(PacketBitReader reader)
    {
        _castleId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Castle castle = CastleManager.getInstance().getCastleById(_castleId);
        if (castle == null)
            return ValueTask.CompletedTask;
		
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        player.sendPacket(new SiegeDefenderListPacket(castle));
        
        return ValueTask.CompletedTask;
    }
}