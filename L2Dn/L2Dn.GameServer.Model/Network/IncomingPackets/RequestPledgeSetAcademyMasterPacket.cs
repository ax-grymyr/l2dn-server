using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeSetAcademyMasterPacket: IIncomingPacket<GameSession>
{
    private string _currPlayerName;
    private string _targetPlayerName;
    private bool _set;

    public void ReadContent(PacketBitReader reader)
    {
        _set = reader.ReadInt32() != 0;
        _currPlayerName = reader.ReadString();
        _targetPlayerName = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null)
		    return ValueTask.CompletedTask;

		Clan? clan = player.getClan();
		if (clan == null)
			return ValueTask.CompletedTask;
		
		if (!player.hasClanPrivilege(ClanPrivilege.CL_APPRENTICE))
		{
			player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_THE_RIGHT_TO_DISMISS_MENTEES);
			return ValueTask.CompletedTask;
		}
		
		ClanMember currentMember = clan.getClanMember(_currPlayerName);
		ClanMember targetMember = clan.getClanMember(_targetPlayerName);
		if (currentMember == null || targetMember == null)
			return ValueTask.CompletedTask;
		
		ClanMember apprenticeMember;
		ClanMember sponsorMember;
		if (currentMember.getPledgeType() == Clan.SUBUNIT_ACADEMY)
		{
			apprenticeMember = currentMember;
			sponsorMember = targetMember;
		}
		else
		{
			apprenticeMember = targetMember;
			sponsorMember = currentMember;
		}
		
		Player apprentice = apprenticeMember.getPlayer();
		Player sponsor = sponsorMember.getPlayer();
		SystemMessagePacket sm;
		if (!_set)
		{
			// test: do we get the current sponsor & apprentice from this packet or no?
			if (apprentice != null)
			{
				apprentice.setSponsor(0);
			}
			else
			{
				apprenticeMember.setApprenticeAndSponsor(0, 0);
			}
			
			if (sponsor != null)
			{
				sponsor.setApprentice(0);
			}
			else
			{
				sponsorMember.setApprenticeAndSponsor(0, 0);
			}
			
			apprenticeMember.saveApprenticeAndSponsor(0, 0);
			sponsorMember.saveApprenticeAndSponsor(0, 0);
			sm = new SystemMessagePacket(SystemMessageId.S2_C1_S_MENTEE_IS_DISMISSED);
		}
		else
		{
			if (apprenticeMember.getSponsor() != 0 || sponsorMember.getApprentice() != 0 || apprenticeMember.getApprentice() != 0 || sponsorMember.getSponsor() != 0)
			{
				// TODO retail message
				player.sendMessage("Remove previous connections first.");
				return ValueTask.CompletedTask;
			}
			if (apprentice != null)
			{
				apprentice.setSponsor(sponsorMember.getObjectId());
			}
			else
			{
				apprenticeMember.setApprenticeAndSponsor(0, sponsorMember.getObjectId());
			}
			
			if (sponsor != null)
			{
				sponsor.setApprentice(apprenticeMember.getObjectId());
			}
			else
			{
				sponsorMember.setApprenticeAndSponsor(apprenticeMember.getObjectId(), 0);
			}
			
			// saving to database even if online, since both must match
			apprenticeMember.saveApprenticeAndSponsor(0, sponsorMember.getObjectId());
			sponsorMember.saveApprenticeAndSponsor(apprenticeMember.getObjectId(), 0);
			sm = new SystemMessagePacket(SystemMessageId.S1_HAS_BECOME_S2_S_MENTOR);
		}
        
		sm.Params.addString(sponsorMember.getName());
		sm.Params.addString(apprenticeMember.getName());
		
		if (sponsor != player && sponsor != apprentice)
		{
			player.sendPacket(sm);
		}
		if (sponsor != null)
		{
			sponsor.sendPacket(sm);
		}
		if (apprentice != null)
		{
			apprentice.sendPacket(sm);
		}

		return ValueTask.CompletedTask;
    }
}