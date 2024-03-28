using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.DailyMissions;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.DailyMissions;

public struct RequestToDoListPacket: IIncomingPacket<GameSession>
{
    private byte _tab;
    private bool _showAllLevels;

    public void ReadContent(PacketBitReader reader)
    {
        _tab = reader.ReadByte(); // Daily Reward = 9, Event = 1, Instance Zone = 2
        _showAllLevels = reader.ReadByte() == 1; // Disabled = 0, Enabled = 1
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        switch (_tab)
        {
            // case 1:
            // {
            // player.sendPacket(new ExTodoListInzone());
            // break;
            // }
            // case 2:
            // {
            // player.sendPacket(new ExTodoListInzone());
            // break;
            // }
            case 9: // Daily Rewards
            {
                // Initial EW request should be false
                ExOneDayReceiveRewardListPacket oneDayReceiveRewardListPacket =
                    new ExOneDayReceiveRewardListPacket(player, true);
                
                connection.Send(ref oneDayReceiveRewardListPacket);
                break;
            }
        }
    
        return ValueTask.CompletedTask;
    }
}