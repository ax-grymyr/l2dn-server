using L2Dn.GameServer.Model.Clans;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnClanLeft: IBaseEvent
{
	private readonly ClanMember _clanMember;
	private readonly Clan _clan;
	
	public OnClanLeft(ClanMember clanMember, Clan clan)
	{
		_clanMember = clanMember;
		_clan = clan;
	}
	
	public ClanMember getClanMember()
	{
		return _clanMember;
	}
	
	public Clan getClan()
	{
		return _clan;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_CLAN_LEFT;
	}
}