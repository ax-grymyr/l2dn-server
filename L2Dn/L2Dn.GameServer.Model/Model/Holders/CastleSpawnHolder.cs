using L2Dn.GameServer.Enums;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author St3eT
 */
public class CastleSpawnHolder
{
	private readonly Location _location;
	private readonly int _npcId;
	private readonly CastleSide _side;

	public CastleSpawnHolder(int npcId, CastleSide side, Location location)
	{
		_location = location;
		_npcId = npcId;
		_side = side;
	}

	public Location Location => _location;

	public int getNpcId()
	{
		return _npcId;
	}

	public CastleSide getSide()
	{
		return _side;
	}
}