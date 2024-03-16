using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;

namespace L2Dn.GameServer.Model.Events.Impl.Clans;

/**
 * @author UnAfraid
 */
public class OnClanLvlUp: EventBase
{
	private readonly Player _player;
	private readonly Clan _clan;
	
	public OnClanLvlUp(Player player, Clan clan)
	{
		_player = player;
		_clan = clan;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public Clan getClan()
	{
		return _clan;
	}
}