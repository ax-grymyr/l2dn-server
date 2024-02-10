using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author St3eT
 */
public class CastleSpawnHolder: Location
{
	private readonly int _npcId;
	private readonly CastleSide _side;

	public CastleSpawnHolder(int npcId, CastleSide side, int x, int y, int z, int heading): base(x, y, z, heading)
	{
		_npcId = npcId;
		_side = side;
	}

	public int getNpcId()
	{
		return _npcId;
	}

	public CastleSide getSide()
	{
		return _side;
	}
}