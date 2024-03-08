using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.SkillConditionHandlers;

/**
 * @author Mobius
 */
public class OpEquipItemSkillCondition: ISkillCondition
{
	private readonly Set<int> _itemIds = new();
	private readonly SkillConditionAffectType _affectType;
	
	public OpEquipItemSkillCondition(StatSet @params)
	{
		List<int> itemIds = @params.getList<int>("itemIds");
		if (itemIds != null)
		{
			_itemIds.addAll(itemIds);
		}
		else
		{
			_itemIds.add(@params.getInt("itemId"));
		}
		_affectType = @params.getEnum<SkillConditionAffectType>("affectType");
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		switch (_affectType)
		{
			case SkillConditionAffectType.CASTER:
			{
				foreach (int itemId in _itemIds)
				{
					if (caster.getInventory().isItemEquipped(itemId))
					{
						return true;
					}
				}
				break;
			}
			case SkillConditionAffectType.TARGET:
			{
				if ((target != null) && target.isPlayer())
				{
					foreach (int itemId in _itemIds)
					{
						if (target.getActingPlayer().getInventory().isItemEquipped(itemId))
						{
							return true;
						}
					}
				}
				break;
			}
			case SkillConditionAffectType.BOTH:
			{
				if ((target != null) && target.isPlayer())
				{
					foreach (int itemId in _itemIds)
					{
						if (target.getActingPlayer().getInventory().isItemEquipped(itemId))
						{
							foreach (int id in _itemIds)
							{
								if (caster.getInventory().isItemEquipped(id))
								{
									return true;
								}
							}
						}
					}
				}
				break;
			}
		}
		return false;
	}
}