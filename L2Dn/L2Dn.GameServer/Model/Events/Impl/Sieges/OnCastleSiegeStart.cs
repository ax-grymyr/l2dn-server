using L2Dn.GameServer.Model.Sieges;

namespace L2Dn.GameServer.Model.Events.Impl.Sieges;

/**
 * @author UnAfraid
 */
public class OnCastleSiegeStart: IBaseEvent
{
	private readonly Siege _siege;
	
	public OnCastleSiegeStart(Siege siege)
	{
		_siege = siege;
	}
	
	public Siege getSiege()
	{
		return _siege;
	}
	
	public EventType getType()
	{
		return EventType.ON_CASTLE_SIEGE_START;
	}
}