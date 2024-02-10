using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Players;

/**
 * @author UnAfraid
 */
public class OnPlayableExpChanged: IBaseEvent
{
	private readonly Playable _playable;
	private readonly long _oldExp;
	private readonly long _newExp;
	
	public OnPlayableExpChanged(Playable playable, long oldExp, long newExp)
	{
		_playable = playable;
		_oldExp = oldExp;
		_newExp = newExp;
	}
	
	public Playable getPlayable()
	{
		return _playable;
	}
	
	public long getOldExp()
	{
		return _oldExp;
	}
	
	public long getNewExp()
	{
		return _newExp;
	}
	
	public EventType getType()
	{
		return EventType.ON_PLAYABLE_EXP_CHANGED;
	}
}