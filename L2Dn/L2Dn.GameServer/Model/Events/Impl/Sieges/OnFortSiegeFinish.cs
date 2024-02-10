using L2Dn.GameServer.Model.Sieges;

namespace L2Dn.GameServer.Model.Events.Impl.Sieges;

/**
 * @author UnAfraid
 */
public class OnFortSiegeFinish: IBaseEvent
{
	private readonly FortSiege _siege;
	
	public OnFortSiegeFinish(FortSiege siege)
	{
		_siege = siege;
	}
	
	public FortSiege getSiege()
	{
		return _siege;
	}
	
	public EventType getType()
	{
		return EventType.ON_FORT_SIEGE_FINISH;
	}
}