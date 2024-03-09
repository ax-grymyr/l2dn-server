using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author nasseka
 */
public class EquipSigilSkillCondition: ISkillCondition
{
	public EquipSigilSkillCondition(StatSet @params)
	{
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		ItemTemplate sigil = caster.getSecondaryWeaponItem();
		return (sigil != null) && (sigil.getItemType() == ArmorType.SIGIL);
	}
}