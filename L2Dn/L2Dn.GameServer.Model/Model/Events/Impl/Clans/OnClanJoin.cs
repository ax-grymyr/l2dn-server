using L2Dn.Events;
using L2Dn.GameServer.Model.Clans;

namespace L2Dn.GameServer.Model.Events.Impl.Clans;

/**
 * @author UnAfraid
 */
public class OnClanJoin: EventBase
{
	private readonly ClanMember _clanMember;
	private readonly Clan _clan;

	public OnClanJoin(ClanMember clanMember, Clan clan)
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
}