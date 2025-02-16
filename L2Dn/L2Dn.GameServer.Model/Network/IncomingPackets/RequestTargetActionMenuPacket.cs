using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestTargetActionMenuPacket: IIncomingPacket<GameSession>
{
    private int _objectId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
        reader.ReadInt16(); // action?
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        // TODO flood protection
        // if (!client.getFloodProtectors().canPerformPlayerAction())
        // {
        //     return;
        // }
		
        Player? player = session.Player;
        if ((player == null) || player.isTargetingDisabled())
            return ValueTask.CompletedTask;
		
        foreach (WorldObject obj in World.getInstance().getVisibleObjects<WorldObject>(player))
        {
            if (_objectId == obj.ObjectId)
            {
                if (obj.isTargetable() && obj.isAutoAttackable(player))
                {
                    player.setTarget(obj);
                }

                break;
            }
        }

        return ValueTask.CompletedTask;
    }
}