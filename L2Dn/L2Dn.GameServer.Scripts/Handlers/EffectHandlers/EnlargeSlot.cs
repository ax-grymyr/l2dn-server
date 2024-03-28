using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class EnlargeSlot: AbstractEffect
{
	private readonly StorageType _type;
	private readonly double _amount;
	
	public EnlargeSlot(StatSet @params)
	{
		_amount = @params.getDouble("amount", 0);
		_type = @params.getEnum("type", StorageType.INVENTORY_NORMAL);
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		Stat stat = Stat.INVENTORY_NORMAL;
		
		switch (_type)
		{
			case StorageType.TRADE_BUY:
			{
				stat = Stat.TRADE_BUY;
				break;
			}
			case StorageType.TRADE_SELL:
			{
				stat = Stat.TRADE_SELL;
				break;
			}
			case StorageType.RECIPE_DWARVEN:
			{
				stat = Stat.RECIPE_DWARVEN;
				break;
			}
			case StorageType.RECIPE_COMMON:
			{
				stat = Stat.RECIPE_COMMON;
				break;
			}
			case StorageType.STORAGE_PRIVATE:
			{
				stat = Stat.STORAGE_PRIVATE;
				break;
			}
		}
		
		effected.getStat().mergeAdd(stat, _amount);
		if (effected.isPlayer())
		{
			effected.getActingPlayer().sendStorageMaxCount();
		}
	}
}