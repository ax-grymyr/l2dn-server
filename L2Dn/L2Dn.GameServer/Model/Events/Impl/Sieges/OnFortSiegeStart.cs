using L2Dn.GameServer.Model.Sieges;

namespace L2Dn.GameServer.Model.Events.Impl.Sieges;

/**
 * @author UnAfraid
 */
public class OnFortSiegeStart: IBaseEvent
{
	private readonly FortSiege _siege;
	
	public OnFortSiegeStart(FortSiege siege)
	{
		_siege = siege;
	}
	
	public FortSiege getSiege()
	{
		return _siege;
	}
	
	public EventType getType()
	{
		return EventType.ON_FORT_SIEGE_START;
	}
}