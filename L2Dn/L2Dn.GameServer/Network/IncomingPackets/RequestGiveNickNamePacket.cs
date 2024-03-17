using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestGiveNickNamePacket: IIncomingPacket<GameSession>
{
    private string _target;
    private string _title;

    public void ReadContent(PacketBitReader reader)
    {
        _target = reader.ReadString();
        _title = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        // Noblesse can bestow a title to themselves
        if (player.isNoble() && _target.equalsIgnoreCase(player.getName()))
        {
            player.setTitle(_title);
            connection.Send(SystemMessageId.YOUR_TITLE_HAS_BEEN_CHANGED);
            player.broadcastTitleInfo();
        }
        else
        {
            // Can the player change/give a title?
            if (!player.hasClanPrivilege(ClanPrivilege.CL_GIVE_TITLE))
            {
                connection.Send(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
                return ValueTask.CompletedTask;
            }
			
            if (player.getClan().getLevel() < 3)
            {
                connection.Send(SystemMessageId.A_PLAYER_CAN_ONLY_BE_GRANTED_A_TITLE_IF_THE_CLAN_IS_LEVEL_3_OR_ABOVE);
                return ValueTask.CompletedTask;
            }
			
            ClanMember member1 = player.getClan().getClanMember(_target);
            if (member1 != null)
            {
                Player member = member1.getPlayer();
                if (member != null)
                {
                    // is target from the same clan?
                    member.setTitle(_title);
                    member.sendPacket(SystemMessageId.YOUR_TITLE_HAS_BEEN_CHANGED);
                    member.broadcastTitleInfo();
                }
                else
                {
                    connection.Send(SystemMessageId.THAT_PLAYER_IS_NOT_ONLINE);
                }
            }
            else
            {
                connection.Send(SystemMessageId.THE_TARGET_MUST_BE_A_CLAN_MEMBER);
            }
        }

        return ValueTask.CompletedTask;
    }
}