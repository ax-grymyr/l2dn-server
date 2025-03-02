using L2Dn.GameServer.Db;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Mentoring;

public struct ConfirmMenteeAddPacket: IIncomingPacket<GameSession>
{
    private int _confirmed;
    private string _mentor;

    public void ReadContent(PacketBitReader reader)
    {
        _confirmed = reader.ReadInt32();
        _mentor = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? mentee = session.Player;
        if (mentee == null)
            return ValueTask.CompletedTask;

        Player? mentor = World.getInstance().getPlayer(_mentor);
        if (mentor == null)
            return ValueTask.CompletedTask;

        if (_confirmed == 0)
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_DECLINED_S1_S_MENTORING_OFFER);
            sm.Params.addString(mentor.getName());
            mentee.sendPacket(sm);

            sm = new SystemMessagePacket(SystemMessageId.S1_HAS_DECLINED_BECOMING_YOUR_MENTEE);
            sm.Params.addString(mentee.getName());
            mentor.sendPacket(sm);
        }
        else if (validate(mentor, mentee))
        {
            try
            {
                using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
                ctx.CharacterMentees.Add(new DbCharacterMentee()
                {
                    CharacterId = mentee.ObjectId,
                    MentorId = mentor.ObjectId
                });

                ctx.SaveChanges();

                MentorManager.getInstance().addMentor(mentor.ObjectId, mentee.ObjectId);

                // Notify to scripts
                if (mentor.Events.HasSubscribers<OnPlayerMenteeAdd>())
                {
                    mentor.Events.NotifyAsync(new OnPlayerMenteeAdd(mentor, mentee));
                }

                SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.FROM_NOW_ON_S1_WILL_BE_YOUR_MENTEE);
                sm.Params.addString(mentee.getName());
                mentor.sendPacket(sm);

                sm = new SystemMessagePacket(SystemMessageId.FROM_NOW_ON_S1_WILL_BE_YOUR_MENTOR);
                sm.Params.addString(mentor.getName());
                mentee.sendPacket(sm);
            }
            catch (Exception e)
            {
                PacketLogger.Instance.Warn(GetType().Name + ": " + e);
            }
        }

        return ValueTask.CompletedTask;
    }

    public static bool validate(Player mentor, Player mentee)
    {
        return false;
    }
}