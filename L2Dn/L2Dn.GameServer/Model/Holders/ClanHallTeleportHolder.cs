namespace L2Dn.GameServer.Model.Holders;

/**
 * @author St3eT
 */
public class ClanHallTeleportHolder: Location
{
	private readonly NpcStringId _npcStringId;
	private readonly int _minFunctionLevel;
	private readonly int _cost;

	public ClanHallTeleportHolder(int npcStringId, int x, int y, int z, int minFunctionLevel, int cost): base(x, y, z)
	{
		_npcStringId = NpcStringId.getNpcStringId(npcStringId);
		_minFunctionLevel = minFunctionLevel;
		_cost = cost;
	}

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