using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpEquipItem")]
public sealed class OpEquipItemSkillCondition: ISkillCondition
{
    private readonly FrozenSet<int> _itemIds;
    private readonly SkillConditionAffectType _affectType;

    public OpEquipItemSkillCondition(SkillConditionParameterSet parameters)
    {
        List<int> itemIds = parameters.GetInt32ListOptional(XmlSkillConditionParameterType.ItemIds) ??
            [parameters.GetInt32(XmlSkillConditionParameterType.ItemId)];

        _itemIds = itemIds.ToFrozenSet();
        _affectType = parameters.GetEnum<SkillConditionAffectType>(XmlSkillConditionParameterType.AffectType);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        switch (_affectType)
        {
            case SkillConditionAffectType.CASTER:
            {
                Inventory? inventory = caster.getInventory();
                if (inventory != null)
                {
                    foreach (int itemId in _itemIds)
                    {
                        if (inventory.isItemEquipped(itemId))
                            return true;
                    }
                }

                break;
            }
            case SkillConditionAffectType.TARGET:
            {
                Player? player = target?.getActingPlayer();
                if (target != null && target.isPlayer() && player != null)
                {
                    foreach (int itemId in _itemIds)
                    {
                        if (player.getInventory().isItemEquipped(itemId))
                        {
                            return true;
                        }
                    }
                }

                break;
            }
            case SkillConditionAffectType.BOTH:
            {
                Player? player = target?.getActingPlayer();
                Inventory? casterInventory = caster.getInventory();
                if (target != null && target.isPlayer() && player != null && casterInventory != null)
                {
                    foreach (int itemId in _itemIds)
                    {
                        if (player.getInventory().isItemEquipped(itemId))
                        {
                            foreach (int id in _itemIds)
                            {
                                if (casterInventory.isItemEquipped(id))
                                    return true;
                            }
                        }
                    }
                }

                break;
            }
        }

        return false;
    }

    public override int GetHashCode() => HashCode.Combine(_itemIds.GetSetHashCode(), _affectType);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._itemIds.GetSetComparable(), x._affectType));
}