using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestMagicSkillListPacket: IIncomingPacket<GameSession>
{
    private int _objectId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
        // packet.readInt(); _charId
        // packet.readInt(); _unk
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (player.getObjectId() != _objectId)
        {
            PacketLogger.Instance.Warn("Player: " + player + " requested " + GetType().Name +
                                       " with different object id: " + _objectId);
            
            return ValueTask.CompletedTask;
        }
		
        player.sendSkillList();
        return ValueTask.CompletedTask;
    }
}