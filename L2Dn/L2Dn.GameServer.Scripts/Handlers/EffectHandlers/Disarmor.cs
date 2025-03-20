using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Disarm by inventory slot effect implementation. At end of effect, it re-equips that item.
/// </summary>
[HandlerName("Disarmor")]
public sealed class Disarmor: AbstractEffect
{
    private readonly Map<int, int> _unequippedItems; // PlayerObjId, ItemObjId
    private readonly long _slot;

    public Disarmor(EffectParameterSet parameters)
    {
        _unequippedItems = [];

        string slot = parameters.GetString(XmlSkillEffectParameterType.Slot, "chest");
        _slot = ItemData._slotNameMap.GetValueOrDefault(slot, ItemTemplate.SLOT_NONE);
        if (_slot == ItemTemplate.SLOT_NONE)
        {
            Logger.Error("Unknown bodypart slot for effect: " + slot);
        }
    }

    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        return _slot != ItemTemplate.SLOT_NONE && effected.isPlayer();
    }

    public override void ContinuousInstant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (!effected.isPlayer() || player == null)
        {
            return;
        }

        List<Item> unequipped = player.getInventory().unEquipItemInBodySlotAndRecord(_slot);
        if (unequipped.Count != 0)
        {
            InventoryUpdatePacket iu =
                new InventoryUpdatePacket(unequipped.Select(x => new ItemInfo(x, ItemChangeType.MODIFIED)).ToList());

            player.sendInventoryUpdate(iu);
            player.broadcastUserInfo();

            SystemMessagePacket sm;
            Item unequippedItem = unequipped[0];
            if (unequippedItem.getEnchantLevel() > 0)
            {
                sm = new SystemMessagePacket(SystemMessageId.S1_S2_UNEQUIPPED);
                sm.Params.addInt(unequippedItem.getEnchantLevel());
                sm.Params.addItemName(unequippedItem);
            }
            else
            {
                sm = new SystemMessagePacket(SystemMessageId.S1_UNEQUIPPED);
                sm.Params.addItemName(unequippedItem);
            }

            player.sendPacket(sm);
            player.getInventory().blockItemSlot(_slot);
            _unequippedItems.put(effected.ObjectId, unequippedItem.ObjectId);
        }
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        Player? player = effected.getActingPlayer();
        if (!effected.isPlayer() || player == null)
            return;

        int disarmedObjId = _unequippedItems.remove(effected.ObjectId);
        if (disarmedObjId > 0)
        {
            player.getInventory().unblockItemSlot(_slot);

            Item? item = player.getInventory().getItemByObjectId(disarmedObjId);
            if (item != null)
            {
                player.getInventory().equipItem(item);
                InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(item, ItemChangeType.MODIFIED));
                player.sendInventoryUpdate(iu);
                if (item.isEquipped())
                {
                    SystemMessagePacket sm;
                    if (item.getEnchantLevel() > 0)
                    {
                        sm = new SystemMessagePacket(SystemMessageId.S1_S2_EQUIPPED);
                        sm.Params.addInt(item.getEnchantLevel());
                        sm.Params.addItemName(item);
                    }
                    else
                    {
                        sm = new SystemMessagePacket(SystemMessageId.S1_EQUIPPED);
                        sm.Params.addItemName(item);
                    }

                    player.sendPacket(sm);
                }
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(_slot);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._slot);
}