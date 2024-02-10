namespace L2Dn.GameServer.Model.Holders;

/**
 * Simple class for storing info for Selling Buffs system.
 * @author St3eT
 */
public class SellBuffHolder
{
	private readonly int _skillId;
	private long _price;

	public SellBuffHolder(int skillId, long price)
	{
		_skillId = skillId;
		_price = price;
	}

	public int getSkillId()
	{
		return _skillId;
	}

	public void setPrice(int price)
	{
		_price = price;
	}

	public long getPrice()
	{
		return _price;
	}
}