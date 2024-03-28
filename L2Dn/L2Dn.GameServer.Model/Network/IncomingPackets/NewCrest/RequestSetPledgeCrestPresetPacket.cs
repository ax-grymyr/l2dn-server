using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.NewCrest;

public struct RequestSetPledgeCrestPresetPacket: IIncomingPacket<GameSession>
{
    private int _emblemType;
    private int _emblem;

    public void ReadContent(PacketBitReader reader)
    {
        _emblemType = reader.ReadInt32();
        _emblem = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Clan clan = player.getClan();
        if (clan == null)
            return ValueTask.CompletedTask;
		
        if (clan.getLevel() < 3)
            return ValueTask.CompletedTask;
		
        if (clan.getLeader().getObjectId() == player.getObjectId())
        {
            if (_emblemType == 0)
            {
                clan.changeClanCrest(0);
                return ValueTask.CompletedTask;
            }
			
            if (_emblemType == 1)
            {
                clan.changeClanCrest(_emblem);
            }
        }
        
        return ValueTask.CompletedTask;
    }
}