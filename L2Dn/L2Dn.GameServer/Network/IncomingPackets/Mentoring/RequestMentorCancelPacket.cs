using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Mentoring;

public struct RequestMentorCancelPacket: IIncomingPacket<GameSession>
{
    private int _confirmed;
    private string _name;

    public void ReadContent(PacketBitReader reader)
    {
        _confirmed = reader.ReadInt32();
        _name = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		if (_confirmed != 1)
			return ValueTask.CompletedTask;
		
		int objectId = CharInfoTable.getInstance().getIdByName(_name);
			if (player.isMentor())
			{
				Mentee mentee = MentorManager.getInstance().getMentee(player.getObjectId(), objectId);
				if (mentee != null)
				{
					MentorManager.getInstance().cancelAllMentoringBuffs(mentee.getPlayer());
					
					if (MentorManager.getInstance().isAllMenteesOffline(player.getObjectId(), mentee.getObjectId()))
					{
						MentorManager.getInstance().cancelAllMentoringBuffs(player);
					}

					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_S_MENTORING_CONTRACT_IS_CANCELLED_THE_MENTOR_CANNOT_BOND_WITH_ANOTHER_MENTEE_FOR_2_DAYS);
					sm.Params.addString(_name);
					player.sendPacket(sm);
					
					MentorManager.getInstance().setPenalty(player.getObjectId(), TimeSpan.FromDays(Config.MENTOR_PENALTY_FOR_MENTEE_LEAVE)); // TODO: verify period
					MentorManager.getInstance().deleteMentor(player.getObjectId(), mentee.getObjectId());
					
					// Notify to scripts
					if (player.Events.HasSubscribers<OnPlayerMenteeRemove>())
					{
						player.Events.NotifyAsync(new OnPlayerMenteeRemove(player, mentee));
					}
				}
			}
			else if (player.isMentee())
			{
				Mentee mentor = MentorManager.getInstance().getMentor(player.getObjectId());
				if ((mentor != null) && (mentor.getObjectId() == objectId))
				{
					MentorManager.getInstance().cancelAllMentoringBuffs(player);
					
					if (MentorManager.getInstance().isAllMenteesOffline(mentor.getObjectId(), player.getObjectId()))
					{
						MentorManager.getInstance().cancelAllMentoringBuffs(mentor.getPlayer());
					}
					
					MentorManager.getInstance().setPenalty(mentor.getObjectId(), TimeSpan.FromDays(Config.MENTOR_PENALTY_FOR_MENTEE_LEAVE)); // TODO: verify period
					MentorManager.getInstance().deleteMentor(mentor.getObjectId(), player.getObjectId());
					
					// Notify to scripts
					if (player.Events.HasSubscribers<OnPlayerMenteeLeft>())
					{
						player.Events.NotifyAsync(new OnPlayerMenteeLeft(mentor, player));
					}
					
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_S_MENTORING_CONTRACT_IS_CANCELLED_THE_MENTOR_CANNOT_BOND_WITH_ANOTHER_MENTEE_FOR_2_DAYS);
					sm.Params.addString(_name);
					mentor.getPlayer().sendPacket(sm);
				}
			}
       
        return ValueTask.CompletedTask;
    }
}