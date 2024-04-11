using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public class Op1hWeaponSkillCondition: ISkillCondition
{
    private readonly Set<WeaponType> _weaponTypes = new();
	
    public Op1hWeaponSkillCondition(StatSet @params)
    {
        List<String> weaponTypes = @params.getList<string>("weaponType");
        if (weaponTypes != null)
        {
            foreach (String type in weaponTypes)
            {
                _weaponTypes.add(Enum.Parse<WeaponType>(type));
            }
        }
    }
	
    public bool canUse(Creature caster, Skill skill, WorldObject target)
    {
        Weapon weapon = caster.getActiveWeaponItem();
        if (weapon == null)
        {
            return false;
        }
		
        foreach (WeaponType weaponType in _weaponTypes)
        {
            if (weapon.getItemType() == weaponType)
            {
                return (weapon.getBodyPart() & ItemTemplate.SLOT_LR_HAND) == 0;
            }
        }
		
        return false;
    }
}