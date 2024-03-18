using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestGmCommandPacket: IIncomingPacket<GameSession>
{
    private string _targetName;
    private int _command;

    public void ReadContent(PacketBitReader reader)
    {
        _targetName = reader.ReadString();
        _command = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        // prevent non GM or low level GMs from viewing player stuff
        Player? thisPlayer = session.Player;
        if (thisPlayer is null)
            return ValueTask.CompletedTask;
        
        if (!thisPlayer.isGM() || !thisPlayer.getAccessLevel().allowAltG())
            return ValueTask.CompletedTask;
		
        Player player = World.getInstance().getPlayer(_targetName);
        Clan clan = ClanTable.getInstance().getClanByName(_targetName);
		
        // player name was incorrect?
        if (player == null && (clan == null || _command != 6))
            return ValueTask.CompletedTask;
		
        switch (_command)
        {
            case 1: // player status
            {
                connection.Send(new GMViewCharacterInfoPacket(player));
                connection.Send(new GMHennaInfoPacket(player));
                break;
            }
            case 2: // player clan
            {
                if (player != null && player.getClan() != null)
                {
                    connection.Send(new GMViewPledgeInfoPacket(player.getClan(), player));
                }
                break;
            }
            case 3: // player skills
            {
                connection.Send(new GMViewSkillInfoPacket(player));
                break;
            }
            case 4: // player quests
            {
                connection.Send(new GMViewQuestInfoPacket(player));
                break;
            }
            case 5: // player inventory
            {
                connection.Send(new GMViewItemListPacket(1, player));
                connection.Send(new GMViewItemListPacket(2, player));
                connection.Send(new GMHennaInfoPacket(player));
                break;
            }
            case 6: // player warehouse
            {
                // GM warehouse view to be implemented
                if (player != null)
                {
                    connection.Send(new GMViewWarehouseWithdrawListPacket(1, player));
                    connection.Send(new GMViewWarehouseWithdrawListPacket(2, player));
                    // clan warehouse
                }
                else
                {
                    connection.Send(new GMViewWarehouseWithdrawListPacket(1, clan));
                    connection.Send(new GMViewWarehouseWithdrawListPacket(2, clan));
                }
                
                break;
            }
        }

        return ValueTask.CompletedTask;
    }
}