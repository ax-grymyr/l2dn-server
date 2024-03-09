using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class OpChangeWeaponSkillCondition: ISkillCondition
{
	public OpChangeWeaponSkillCondition(StatSet @params)
	{
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		Weapon weaponItem = caster.getActiveWeaponItem();
		if (weaponItem == null)
		{
			return false;
		}
		
		if (weaponItem.getChangeWeaponId() == 0)
		{
			return false;
		}
		
		if (caster.getActingPlayer().hasItemRequest())
		{
			return false;
		}
		return true;
	}
}