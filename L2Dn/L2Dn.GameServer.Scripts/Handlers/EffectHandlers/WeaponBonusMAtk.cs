using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("WeaponBonusMAtk")]
public sealed class WeaponBonusMAtk(EffectParameterSet parameters):
    AbstractStatAddEffect(parameters, Stat.WEAPON_BONUS_MAGIC_ATTACK)
{
    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        Item? weapon = player.getInventory().getPaperdollItem(Inventory.PAPERDOLL_RHAND);
        if (weapon == null)
            return;

        InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(weapon, ItemChangeType.MODIFIED));
        player.sendInventoryUpdate(iu);
        player.broadcastUserInfo();
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        Item? weapon = player.getInventory().getPaperdollItem(Inventory.PAPERDOLL_RHAND);
        if (weapon == null)
            return;

        InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(weapon, ItemChangeType.MODIFIED));
        player.sendInventoryUpdate(iu);
        player.broadcastUserInfo();
    }
}