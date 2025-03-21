using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Sdw
 */
public class NpcLogListHolder
{
	private readonly NpcStringId _id;
	private readonly bool _isNpcString;
	private readonly int _count;

	public NpcLogListHolder(NpcStringId npcStringId, int count)
	{
		_id = npcStringId;
		_isNpcString = true;
		_count = count;
	}

	public NpcLogListHolder(NpcStringId id, bool isNpcString, int count)
	{
		_id = id;
		_isNpcString = isNpcString;
		_count = count;
	}

	public NpcStringId getId()
	{
		return _id;
	}

	public bool isNpcString()
	{
		return _isNpcString;
	}

	public int getCount()
	{
		return _count;
	}
}