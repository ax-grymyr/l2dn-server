using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerClanLvlUp: IBaseEvent
{
	private readonly Player _player;
	private readonly Clan _clan;
	
	public OnPlayerClanLvlUp(Player player, Clan clan)
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
	
	public EventType getType()
	{
		return EventType.ON_PLAYER_CLAN_LEVELUP;
	}
}