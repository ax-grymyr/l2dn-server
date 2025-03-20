using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerName("EquipArmor")]
public sealed class EquipArmorSkillCondition: ISkillCondition
{
    private readonly ItemTypeMask _armorTypesMask = ItemTypeMask.Zero;

    public EquipArmorSkillCondition(SkillConditionParameterSet parameters)
    {
        List<string>? armorTypes = parameters.GetStringListOptional(XmlSkillConditionParameterType.ArmorType);
        if (armorTypes != null)
        {
            foreach (string armorType in armorTypes)
                _armorTypesMask |= Enum.Parse<ArmorType>(armorType, true);
        }
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        if (caster == null || !caster.isPlayer())
            return false;

        // Get the itemMask of the weared chest (if exists).
        Inventory? inv = caster.getInventory();
        Item? chest = inv?.getPaperdollItem(Inventory.PAPERDOLL_CHEST);
        if (chest == null)
            return false;

        // If chest armor is different from the condition one return false.
        ItemTypeMask chestMask = chest.getTemplate().getItemMask();
        if ((_armorTypesMask & chestMask) == ItemTypeMask.Zero)
            return false;

        // So from here, chest armor matches conditions.
        // Return True if chest armor is a Full Armor.
        long chestBodyPart = chest.getTemplate().getBodyPart();
        if (chestBodyPart == ItemTemplate.SLOT_FULL_ARMOR)
            return true;

        // Check legs armor.
        Item? legs = inv?.getPaperdollItem(Inventory.PAPERDOLL_LEGS);
        if (legs == null)
            return false;

        // Return true if legs armor matches too.
        ItemTypeMask legMask = legs.getTemplate().getItemMask();
        return (_armorTypesMask & legMask) != ItemTypeMask.Zero;
    }
}