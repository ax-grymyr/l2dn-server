using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.OutgoingPackets.Teleports;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Teleports;

public struct ExRequestSharedLocationTeleportUiPacket: IIncomingPacket<GameSession>
{
    private int _id;

    public void ReadContent(PacketBitReader reader)
    {
        _id = (reader.ReadInt32() - 1) / 256; // TODO: strange arithmetic, maybe just 1 byte must be skipped
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        SharedTeleportHolder teleport = SharedTeleportManager.getInstance().getTeleport(_id);
        if (teleport == null)
            return ValueTask.CompletedTask;
		
        player.sendPacket(new ExShowSharedLocationTeleportUiPacket(teleport));
        
        return ValueTask.CompletedTask;
    }
}