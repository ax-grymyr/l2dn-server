using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Items;

namespace L2Dn.GameServer.Model;

public class Seed
{
	private readonly int _seedId;
	private readonly int _cropId; // crop type
	private readonly int _level; // seed level
	private readonly int _matureId; // mature crop type
	private readonly int _reward1;
	private readonly int _reward2;
	private readonly int _castleId; // id of manor (castle id) where seed can be farmed
	private readonly bool _isAlternative;
	private readonly int _limitSeeds;
	private readonly int _limitCrops;
	private readonly int _seedReferencePrice;
	private readonly int _cropReferencePrice;
	
	public Seed(StatSet set)
	{
		_cropId = set.getInt("id");
		_seedId = set.getInt("seedId");
		_level = set.getInt("level");
		_matureId = set.getInt("mature_Id");
		_reward1 = set.getInt("reward1");
		_reward2 = set.getInt("reward2");
		_castleId = set.getInt("castleId");
		_isAlternative = set.getBoolean("alternative");
		_limitCrops = set.getInt("limit_crops");
		_limitSeeds = set.getInt("limit_seed");
		
		// Set prices
		ItemTemplate item = ItemData.getInstance().getTemplate(_cropId);
		_cropReferencePrice = (item != null) ? item.getReferencePrice() : 1;
		item = ItemData.getInstance().getTemplate(_seedId);
		_seedReferencePrice = (item != null) ? item.getReferencePrice() : 1;
	}
	
	public int getCastleId()
	{
		return _castleId;
	}
	
	public int getSeedId()
	{
		return _seedId;
	}
	
	public int getCropId()
	{
		return _cropId;
	}
	
	public int getMatureId()
	{
		return _matureId;
	}
	
	public int getReward(int type)
	{
		return (type == 1) ? _reward1 : _reward2;
	}
	
	public int getLevel()
	{
		return _level;
	}
	
	public bool isAlternative()
	{
		return _isAlternative;
	}
	
	public int getSeedLimit()
	{
		return _limitSeeds * Config.RATE_DROP_MANOR;
	}
	
	public int getCropLimit()
	{
		return _limitCrops * Config.RATE_DROP_MANOR;
	}
	
	public int getSeedReferencePrice()
	{
		return _seedReferencePrice;
	}
	
	public int getSeedMaxPrice()
	{
		return _seedReferencePrice * 10;
	}
	
	public int getSeedMinPrice()
	{
		return (int) (_seedReferencePrice * 0.6);
	}
	
	public int getCropReferencePrice()
	{
		return _cropReferencePrice;
	}
	
	public int getCropMaxPrice()
	{
		return _cropReferencePrice * 10;
	}
	
	public int getCropMinPrice()
	{
		return (int) (_cropReferencePrice * 0.6);
	}
	
	public override string ToString()
	{
		return "SeedData [_id=" + _seedId + ", _level=" + _level + ", _crop=" + _cropId + ", _mature=" + _matureId + ", _type1=" + _reward1 + ", _type2=" + _reward2 + ", _manorId=" + _castleId + ", _isAlternative=" + _isAlternative + ", _limitSeeds=" + _limitSeeds + ", _limitCrops=" + _limitCrops + "]";
	}
}