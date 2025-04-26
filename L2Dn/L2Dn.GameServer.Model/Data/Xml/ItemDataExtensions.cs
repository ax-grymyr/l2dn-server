using System.Text;
using L2Dn.Events;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Events.Impl.Items;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Data.Xml;

public static class ItemDataExtensions
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ItemDataExtensions));

    /**
     * Create the Item corresponding to the Item Identifier and quantity add logs the activity. <b><u>Actions</u>:</b>
     * <li>Create and Init the Item corresponding to the Item Identifier and quantity</li>
     * <li>Add the Item object to _allObjects of L2world</li>
     * <li>Logs Item creation according to log settings</li><br>
     * @param process : String Identifier of process triggering this action
     * @param itemId : int Item Identifier of the item to be created
     * @param count : int Quantity of items to be created for stackable items
     * @param actor : Creature requesting the item creation
     * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
     * @return Item corresponding to the new item
     */
    public static Item createItem(string process, int itemId, long count, Creature? actor, object? reference = null)
    {
        // Create and Init the Item corresponding to the Item Identifier
        Item item = new Item(IdManager.getInstance().getNextId(), itemId);
        if (process.equalsIgnoreCase("loot") && !Config.Character.AUTO_LOOT_ITEM_IDS.Contains(itemId))
        {
            ScheduledFuture itemLootShedule;
            if (reference is Attackable && ((Attackable)reference).isRaid()) // loot privilege for raids
            {
                Attackable raid = (Attackable)reference;
                // if in CommandChannel and was killing a World/RaidBoss
                CommandChannel? firstCommandChannelAttacked = raid.getFirstCommandChannelAttacked();
                if (firstCommandChannelAttacked != null && !Config.Character.AUTO_LOOT_RAIDS)
                {
                    item.setOwnerId(firstCommandChannelAttacked.getLeaderObjectId());
                    itemLootShedule = ThreadPool.schedule(new ResetOwner(item),
                        Config.Character.LOOT_RAIDS_PRIVILEGE_INTERVAL);

                    item.setItemLootShedule(itemLootShedule);
                }
            }
            else if (!Config.Character.AUTO_LOOT ||
                     (reference is EventMonster && ((EventMonster)reference).eventDropOnGround()))
            {
                item.setOwnerId(actor?.ObjectId ?? 0); // TODO: assign owner id
                itemLootShedule = ThreadPool.schedule(new ResetOwner(item), 15000);
                item.setItemLootShedule(itemLootShedule);
            }
        }

        // Add the Item object to _allObjects of L2world
        World.getInstance().addObject(item);

        // Set Item parameters
        if (item.isStackable() && count > 1)
        {
            item.setCount(count);
        }

        if ((Config.General.LOG_ITEMS && !process.equals("Reset") &&
                !Config.General.LOG_ITEMS_SMALL_LOG && !Config.General.LOG_ITEMS_IDS_ONLY) ||
            (Config.General.LOG_ITEMS_SMALL_LOG && (item.isEquipable() || item.Id == Inventory.AdenaId)) ||
            (Config.General.LOG_ITEMS_IDS_ONLY && Config.General.LOG_ITEMS_IDS_LIST.Contains(item.Id)))
        {
            if (item.getEnchantLevel() > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("CREATE:");
                sb.Append(process);
                sb.Append(", item ");
                sb.Append(item.ObjectId);
                sb.Append(":+");
                sb.Append(item.getEnchantLevel());
                sb.Append(" ");
                sb.Append(item.getTemplate().getName());
                sb.Append("(");
                sb.Append(item.getCount());
                sb.Append("), ");
                sb.Append(actor);
                sb.Append(", ");
                sb.Append(reference);
                _logger.Info(sb.ToString());
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("CREATE:");
                sb.Append(process);
                sb.Append(", item ");
                sb.Append(item.ObjectId);
                sb.Append(":");
                sb.Append(item.getTemplate().getName());
                sb.Append("(");
                sb.Append(item.getCount());
                sb.Append("), ");
                sb.Append(actor);
                sb.Append(", ");
                sb.Append(reference);
                _logger.Info(sb.ToString());
            }
        }

        if (actor != null && actor is Player player && player.isGM() && Config.General.GMAUDIT)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(process);
            sb.Append("(id: ");
            sb.Append(itemId);
            sb.Append(" count: ");
            sb.Append(count);
            sb.Append(" name: ");
            sb.Append(item.getItemName());
            sb.Append(" objId: ");
            sb.Append(item.ObjectId);
            sb.Append(")");

            WorldObject? actorTarget = actor.getTarget();
            string targetName = actorTarget != null ? actorTarget.getName() : "no-target";

            string referenceName = "no-reference";
            if (reference is WorldObject)
            {
                referenceName = ((WorldObject)reference).getName() != null
                    ? ((WorldObject)reference).getName()
                    : "no-name";
            }
            else if (reference is string)
            {
                referenceName = (string)reference;
            }

            // TODO: GMAudit
            // GMAudit.auditGMAction(actor.ToString(), sb.ToString(), targetName,
            // 	StringUtil.concat("Object referencing this action is: ", referenceName));
        }

        // Notify to scripts
        EventContainer itemEvents = item.getTemplate().Events;
        if (itemEvents.HasSubscribers<OnItemCreate>())
        {
            itemEvents.NotifyAsync(new OnItemCreate(process, item, actor, reference));
        }

        return item;
    }

    /**
     * Destroys the Item.<br>
     * <br>
     * <b><u>Actions</u>:</b>
     * <ul>
     * <li>Sets Item parameters to be unusable</li>
     * <li>Removes the Item object to _allObjects of L2world</li>
     * <li>Logs Item deletion according to log settings</li>
     * </ul>
     * @param process a string identifier of process triggering this action.
     * @param item the item instance to be destroyed.
     * @param actor the player requesting the item destroy.
     * @param reference the object referencing current action like NPC selling item or previous item in transformation.
     */
    public static void destroyItem(string? process, Item item, Player? actor, object? reference)
    {
        lock (item)
        {
            long old = item.getCount();
            item.setCount(0);
            item.setOwnerId(0);
            item.setItemLocation(ItemLocation.VOID);
            item.setLastChange(ItemChangeType.REMOVED);

            World.getInstance().removeObject(item);
            IdManager.getInstance().releaseId(item.ObjectId);

            if ((Config.General.LOG_ITEMS && !Config.General.LOG_ITEMS_SMALL_LOG &&
                    !Config.General.LOG_ITEMS_IDS_ONLY) ||
                (Config.General.LOG_ITEMS_SMALL_LOG && (item.isEquipable() || item.Id == Inventory.AdenaId)) ||
                (Config.General.LOG_ITEMS_IDS_ONLY && Config.General.LOG_ITEMS_IDS_LIST.Contains(item.Id)))
            {
                if (item.getEnchantLevel() > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("DELETE:");
                    sb.Append(process);
                    sb.Append(", item ");
                    sb.Append(item.ObjectId);
                    sb.Append(":+");
                    sb.Append(item.getEnchantLevel());
                    sb.Append(" ");
                    sb.Append(item.getTemplate().getName());
                    sb.Append("(");
                    sb.Append(item.getCount());
                    sb.Append("), PrevCount(");
                    sb.Append(old);
                    sb.Append("), ");
                    sb.Append(actor);
                    sb.Append(", ");
                    sb.Append(reference);
                    _logger.Info(sb.ToString());
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("DELETE:");
                    sb.Append(process);
                    sb.Append(", item ");
                    sb.Append(item.ObjectId);
                    sb.Append(":");
                    sb.Append(item.getTemplate().getName());
                    sb.Append("(");
                    sb.Append(item.getCount());
                    sb.Append("), PrevCount(");
                    sb.Append(old);
                    sb.Append("), ");
                    sb.Append(actor);
                    sb.Append(", ");
                    sb.Append(reference);
                    _logger.Info(sb.ToString());
                }
            }

            if (actor != null && actor.isGM() && Config.General.GMAUDIT)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(process);
                sb.Append("(id: ");
                sb.Append(item.Id);
                sb.Append(" count: ");
                sb.Append(item.getCount());
                sb.Append(" itemObjId: ");
                sb.Append(item.ObjectId);
                sb.Append(")");

                string targetName = actor.getTarget()?.getName() ?? "no-target";

                string referenceName = "no-reference";
                if (reference is WorldObject)
                {
                    referenceName = ((WorldObject)reference).getName() != null
                        ? ((WorldObject)reference).getName()
                        : "no-name";
                }
                else if (reference is string)
                {
                    referenceName = (string)reference;
                }

                // TODO: GMAudit
                //GMAudit.auditGMAction(actor.ToString(), sb.ToString(), targetName,
                //	StringUtil.concat("Object referencing this action is: ", referenceName));
            }

            // if it's a pet control item, delete the pet as well
            if (item.getTemplate().isPetItem())
            {
                try
                {
                    using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
                    int itemId = item.ObjectId;
                    // Delete the pet in db
                    ctx.Pets.Where(pet => pet.ItemObjectId == itemId).ExecuteDelete();
                }
                catch (Exception e)
                {
                    _logger.Error(nameof(ItemDataExtensions) + ": Could not delete pet objectid:" + e);
                }
            }
        }
    }

    private sealed class ResetOwner(Item item): Runnable
    {
        public void run()
        {
            // Set owner id to 0 only when location is VOID.
            if (item.getItemLocation() == ItemLocation.VOID)
            {
                item.setOwnerId(0);
            }

            item.setItemLootShedule(null);
        }
    }
}