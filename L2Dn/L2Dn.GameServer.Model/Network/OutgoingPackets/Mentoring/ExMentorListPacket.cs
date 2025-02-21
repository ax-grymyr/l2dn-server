using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Mentoring;

public readonly struct ExMentorListPacket: IOutgoingPacket
{
    private enum MentoringType
    {
        None,
        Mentee,
        Mentor,
    }

    private readonly MentoringType _type;
    private readonly ICollection<Mentee> _mentees;

    public ExMentorListPacket(Player player)
    {
        if (player.isMentor())
        {
            _type = MentoringType.Mentee;
            _mentees = MentorManager.getInstance().getMentees(player.ObjectId);
        }
        else if (player.isMentee())
        {
            Mentee? mentor = MentorManager.getInstance().getMentor(player.ObjectId);
            if (mentor == null)
            {
                _type = MentoringType.None;
                _mentees = [];
            }
            else
            {
                _type = MentoringType.Mentor;
                _mentees = [mentor];
            }
        }
        // else if (player.isInCategory(CategoryType.SIXTH_CLASS_GROUP)) // Not a mentor, Not a mentee, so can be a mentor
        // {
        // _mentees = Collections.emptyList();
        // _type = 1;
        // }
        else
        {
            _mentees = [];
            _type = MentoringType.None;
        }
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MENTOR_LIST);

        writer.WriteInt32((int)_type);
        writer.WriteInt32(0);
        writer.WriteInt32(_mentees.Count);
        foreach (Mentee mentee in _mentees)
        {
            writer.WriteInt32(mentee.getObjectId());
            writer.WriteString(mentee.getName());
            writer.WriteInt32((int)mentee.getClassId());
            writer.WriteInt32(mentee.getLevel());
            writer.WriteInt32((int)mentee.isOnlineInt());
        }
    }
}