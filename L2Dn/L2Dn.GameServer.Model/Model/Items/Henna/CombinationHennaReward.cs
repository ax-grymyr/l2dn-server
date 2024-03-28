using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Combination;

namespace L2Dn.GameServer.Model.Items.Henna;

/**
 * @author Index
 */
public class CombinationHennaReward: ItemHolder
{
	private readonly int _hennaId;
	private readonly CombinationItemType _type;

	public CombinationHennaReward(int id, int count, CombinationItemType type): base(id, count)
	{
		_hennaId = 0;
		_type = type;
	}

	public CombinationHennaReward(int hennaId, int id, int count, CombinationItemType type): base(id, count)
	{
		_hennaId = hennaId;
		_type = type;
	}

	public int getHennaId()
	{
		return _hennaId;
	}

	public CombinationItemType getType()
	{
		return _type;
	}
}