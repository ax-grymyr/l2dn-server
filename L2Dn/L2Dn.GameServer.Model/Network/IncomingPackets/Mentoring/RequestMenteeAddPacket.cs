using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Mentoring;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Mentoring;

public struct RequestMenteeAddPacket: IIncomingPacket<GameSession>
{
    private string _target;

    public void ReadContent(PacketBitReader reader)
    {
        _target = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? mentor = session.Player;
        if (mentor == null)
            return ValueTask.CompletedTask;

        Player mentee = World.getInstance().getPlayer(_target);
        if (mentee == null)
            return ValueTask.CompletedTask;
		
        if (ConfirmMenteeAddPacket.validate(mentor, mentee))
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OFFERED_TO_BECOME_S1_S_MENTOR);
            sm.Params.addString(mentee.getName());
            mentor.sendPacket(sm);
            mentee.sendPacket(new ExMentorAddPacket(mentor));
        }
        
        return ValueTask.CompletedTask;
    }
}