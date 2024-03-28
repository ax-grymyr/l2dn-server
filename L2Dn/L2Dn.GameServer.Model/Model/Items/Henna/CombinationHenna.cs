using System.Xml.Linq;
using L2Dn.GameServer.Model.Items.Combination;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Items.Henna;

/**
 * @author Index
 */
public class CombinationHenna
{
	private readonly int _henna;
	private readonly int _itemOne;
	private readonly long _countOne;
	private readonly int _itemTwo;
	private readonly long _countTwo;
	private readonly long _commission;
	private readonly float _chance;
	private readonly Map<CombinationItemType, CombinationHennaReward> _rewards = new();

	public CombinationHenna(XElement element)
	{
		_henna = element.GetAttributeValueAsInt32("dyeId");
		_itemOne = element.Attribute("itemOne").GetInt32(-1);
		_countOne = element.Attribute("countOne").GetInt64(1);
		_itemTwo = element.Attribute("itemTwo").GetInt32(-1);
		_countTwo = element.Attribute("countTwo").GetInt64(1);
		_commission = element.Attribute("commission").GetInt64(0);
		_chance = element.Attribute("chance").GetFloat(33);
	}

	public int getHenna()
	{
		return _henna;
	}

	public int getItemOne()
	{
		return _itemOne;
	}

	public long getCountOne()
	{
		return _countOne;
	}

	public int getItemTwo()
	{
		return _itemTwo;
	}

	public long getCountTwo()
	{
		return _countTwo;
	}

	public long getCommission()
	{
		return _commission;
	}

	public float getChance()
	{
		return _chance;
	}

	public void addReward(CombinationHennaReward item)
	{
		_rewards.put(item.getType(), item);
	}

	public CombinationHennaReward getReward(CombinationItemType type)
	{
		return _rewards.get(type);
	}
}