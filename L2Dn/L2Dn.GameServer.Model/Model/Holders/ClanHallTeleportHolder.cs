using L2Dn.GameServer.Network.Enums;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author St3eT
 */
public class ClanHallTeleportHolder
{
	private readonly Location3D _location;
	private readonly NpcStringId _npcStringId;
	private readonly int _minFunctionLevel;
	private readonly int _cost;

	public ClanHallTeleportHolder(NpcStringId npcStringId, Location3D location, int minFunctionLevel, int cost)
	{
		_location = location;
		_npcStringId = npcStringId;
		_minFunctionLevel = minFunctionLevel;
		_cost = cost;
	}

	public Location3D Location => _location;

	public NpcStringId getNpcStringId()
	{
		return _npcStringId;
	}

	public int getMinFunctionLevel()
	{
		return _minFunctionLevel;
	}

	public int getCost()
	{
		return _cost;
	}
}