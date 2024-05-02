using L2Dn.Geometry;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author St3eT
 */
public class CastleSpawnHolder
{
	private readonly LocationHeading _location;
	private readonly int _npcId;
	private readonly CastleSide _side;

	public CastleSpawnHolder(int npcId, CastleSide side, LocationHeading location)
	{
		_location = location;
		_npcId = npcId;
		_side = side;
	}

	public LocationHeading Location => _location;

	public int getNpcId()
	{
		return _npcId;
	}

	public CastleSide getSide()
	{
		return _side;
	}
}