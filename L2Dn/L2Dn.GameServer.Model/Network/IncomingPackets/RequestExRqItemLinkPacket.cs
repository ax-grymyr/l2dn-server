using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExRqItemLinkPacket: IIncomingPacket<GameSession>
{
    private int _objectId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        WorldObject? obj = World.getInstance().findObject(_objectId);
        if (obj != null && obj.isItem())
        {
            Item item = (Item)obj;
            if (item.isPublished())
            {
                player.sendPacket(new ExRpItemLinkPacket(item));
            }
        }

        return ValueTask.CompletedTask;
    }
}