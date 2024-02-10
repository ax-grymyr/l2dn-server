using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;

/**
 * @author malyelfik
 */
public class OnNpcManorBypass: IBaseEvent
{
	private readonly Player _player;
	private readonly Npc _target;
	private readonly int _request;
	private readonly int _manorId;
	private readonly bool _nextPeriod;

	public OnNpcManorBypass(Player player, Npc target, int request, int manorId, bool nextPeriod)
	{
		_player = player;
		_target = target;
		_request = request;
		_manorId = manorId;
		_nextPeriod = nextPeriod;
	}

	public Player getActiveChar()
	{
		return _player;
	}

	public Npc getTarget()
	{
		return _target;
	}

	public int getRequest()
	{
		return _request;
	}

	public int getManorId()
	{
		return _manorId;
	}

	public bool isNextPeriod()
	{
		return _nextPeriod;
	}

	public EventType getType()
	{
		return EventType.ON_NPC_MANOR_BYPASS;
	}
}