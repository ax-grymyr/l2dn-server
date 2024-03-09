using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author CostyKiller
 */
public class EquippedCloakEnchantSkillCondition: ISkillCondition
{
	private readonly int _amount;
	
	public EquippedCloakEnchantSkillCondition(StatSet @params)
	{
		_amount = @params.getInt("amount", 0);
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		if ((caster == null) || !caster.isPlayer())
		{
			return false;
		}
		
		Item cloak = caster.getInventory().getPaperdollItem(Inventory.PAPERDOLL_CLOAK);
		if (cloak == null)
		{
			return false;
		}
		
		return cloak.getEnchantLevel() >= _amount;
	}
}