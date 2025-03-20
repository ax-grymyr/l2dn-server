using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public sealed class EquippedCloakEnchantSkillCondition: ISkillCondition
{
    private readonly int _amount;

    public EquippedCloakEnchantSkillCondition(SkillConditionParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillConditionParameterType.Amount, 0);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        if (caster == null || !caster.isPlayer())
            return false;

        Item? cloak = caster.getInventory()?.getPaperdollItem(Inventory.PAPERDOLL_CLOAK);
        if (cloak == null)
            return false;

        return cloak.getEnchantLevel() >= _amount;
    }
}