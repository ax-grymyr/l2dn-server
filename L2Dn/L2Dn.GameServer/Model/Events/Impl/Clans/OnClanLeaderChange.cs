using L2Dn.Events;
using L2Dn.GameServer.Model.Clans;

namespace L2Dn.GameServer.Model.Events.Impl.Clans;

/**
 * @author UnAfraid
 */
public class OnClanLeaderChange: EventBase
{
	private readonly ClanMember _oldLeader;
	private readonly ClanMember _newLeader;
	private readonly Clan _clan;
	
	public OnClanLeaderChange(ClanMember oldLeader, ClanMember newLeader, Clan clan)
	{
		_oldLeader = oldLeader;
		_newLeader = newLeader;
		_clan = clan;
	}
	
	public ClanMember getOldLeader()
	{
		return _oldLeader;
	}
	
	public ClanMember getNewLeader()
	{
		return _newLeader;
	}
	
	public Clan getClan()
	{
		return _clan;
	}
}