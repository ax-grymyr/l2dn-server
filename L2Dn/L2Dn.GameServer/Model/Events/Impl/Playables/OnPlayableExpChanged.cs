using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Base;

namespace L2Dn.GameServer.Model.Events.Impl.Playables;

/**
 * @author UnAfraid
 */
public class OnPlayableExpChanged: TerminateEventBase
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
}