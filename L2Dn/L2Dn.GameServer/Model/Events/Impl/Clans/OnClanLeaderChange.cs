using L2Dn.GameServer.Model.Clans;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnClanLeaderChange: IBaseEvent
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
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_CLAN_LEADER_CHANGE;
	}
}