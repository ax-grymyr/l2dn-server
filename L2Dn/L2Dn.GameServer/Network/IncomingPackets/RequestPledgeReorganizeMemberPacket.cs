using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeReorganizeMemberPacket: IIncomingPacket<GameSession>
{
    private int _isMemberSelected;
    private string _memberName;
    private int _newPledgeType;
    private string _selectedMember;

    public void ReadContent(PacketBitReader reader)
    {
        _isMemberSelected = reader.ReadInt32();
        _memberName = reader.ReadString();
        _newPledgeType = reader.ReadInt32();
        _selectedMember = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_isMemberSelected == 0)
            return ValueTask.CompletedTask;
		
        Clan clan = player.getClan();
        if (clan == null)
            return ValueTask.CompletedTask;
		
        if (!player.hasClanPrivilege(ClanPrivilege.CL_MANAGE_RANKS))
            return ValueTask.CompletedTask;
		
        ClanMember member1 = clan.getClanMember(_memberName);
        if (member1 == null || member1.getObjectId() == clan.getLeaderId())
            return ValueTask.CompletedTask;
		
        ClanMember member2 = clan.getClanMember(_selectedMember);
        if (member2 == null || member2.getObjectId() == clan.getLeaderId())
            return ValueTask.CompletedTask;
		
        int oldPledgeType = member1.getPledgeType();
        if (oldPledgeType == _newPledgeType)
            return ValueTask.CompletedTask;
		
        member1.setPledgeType(_newPledgeType);
        member2.setPledgeType(oldPledgeType);
        clan.broadcastClanStatus();
        
        return ValueTask.CompletedTask;
    }
}