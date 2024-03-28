using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author St3eT
 */
public class SiegeGuardHolder
{
	private readonly int _castleId;
	private readonly int _itemId;
	private readonly SiegeGuardType _type;
	private readonly bool _stationary;
	private readonly int _npcId;
	private readonly int _maxNpcAmount;

	public SiegeGuardHolder(int castleId, int itemId, SiegeGuardType type, bool stationary, int npcId, int maxNpcAmount)
	{
		_castleId = castleId;
		_itemId = itemId;
		_type = type;
		_stationary = stationary;
		_npcId = npcId;
		_maxNpcAmount = maxNpcAmount;
	}

	public int getCastleId()
	{
		return _castleId;
	}

	public int getItemId()
	{
		return _itemId;
	}

	public SiegeGuardType getType()
	{
		return _type;
	}

	public bool isStationary()
	{
		return _stationary;
	}

	public int getNpcId()
	{
		return _npcId;
	}

	public int getMaxNpcAmout()
	{
		return _maxNpcAmount;
	}
}