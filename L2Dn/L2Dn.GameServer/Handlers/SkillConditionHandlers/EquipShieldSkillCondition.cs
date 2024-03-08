using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.SkillConditionHandlers;

/**
 * @author UnAfraid
 */
public class EquipShieldSkillCondition: ISkillCondition
{
	public EquipShieldSkillCondition(StatSet @params)
	{
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		ItemTemplate shield = caster.getSecondaryWeaponItem();
		return (shield != null) && (shield.getItemType() == ArmorType.SHIELD);
	}
}