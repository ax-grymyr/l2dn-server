using System.Collections.Immutable;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Model.Items;

public static class ItemTemplateExtensions
{
    public static bool checkCondition(this ItemTemplate self, Creature creature, WorldObject @object, bool sendMessage)
    {
        if (creature.canOverrideCond(PlayerCondOverride.ITEM_CONDITIONS) && !Config.General.GM_ITEM_RESTRICTION)
            return true;

        // Don't allow hero equipment and restricted items during Olympiad
        Player? player = creature.getActingPlayer();
        if ((self.isOlyRestrictedItem() || self.isHeroItem()) && creature.isPlayer() && player != null &&
            player.isInOlympiadMode())
        {
            if (self.isEquipable())
            {
                creature.sendPacket(SystemMessageId.THE_ITEM_CANNOT_BE_EQUIPPED_IN_THE_OLYMPIAD);
            }
            else
            {
                creature.sendPacket(SystemMessageId.THE_ITEM_CANNOT_BE_USED_IN_THE_OLYMPIAD);
            }

            return false;
        }

        if (self.isEventRestrictedItem() && creature.isPlayer() && player != null && player.isOnEvent())
        {
            creature.sendMessage("You cannot use this item in the event.");
            return false;
        }

        ImmutableArray<IConditionBase>? conditions = self.getConditions();
        if (!self.isConditionAttached() || conditions == null)
            return true;

        Creature? target = @object.isCreature() ? (Creature)@object : null;
        foreach (IConditionBase condition in conditions)
        {
            Condition preCondition = (Condition)condition;
            if (!preCondition.test(creature, target))
            {
                if (creature.isSummon())
                {
                    creature.sendPacket(SystemMessageId.THIS_PET_CANNOT_USE_THIS_ITEM);
                    return false;
                }

                if (sendMessage)
                {
                    string? msg = preCondition.getMessage();
                    SystemMessageId msgId = preCondition.getMessageId();
                    if (msg != null)
                    {
                        creature.sendMessage(msg);
                    }
                    else if (msgId != 0)
                    {
                        SystemMessagePacket sm = new SystemMessagePacket(msgId);
                        if (preCondition.isAddName())
                        {
                            sm.Params.addItemName(self.Id);
                        }

                        creature.sendPacket(sm);
                    }
                }

                return false;
            }
        }

        return true;
    }
}