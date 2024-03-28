using L2Dn.GameServer.Model.Actor.Instances;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author UnAfraid
 */
public class DoorRequestHolder
{
	private readonly Door _target;

	public DoorRequestHolder(Door door)
	{
		_target = door;
	}

	public Door getDoor()
	{
		return _target;
	}
}