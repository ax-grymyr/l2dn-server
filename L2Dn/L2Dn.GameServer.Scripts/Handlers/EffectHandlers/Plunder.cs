using L2Dn.GameServer.AI;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class Plunder: AbstractEffect
{
    public Plunder(StatSet @params)
    {
    }

    public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return Formulas.calcMagicSuccess(effector, effected, skill);
    }

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (!effector.isPlayer() || player == null)
            return;

        if (!effected.isMonster())
        {
            effector.sendPacket(SystemMessageId.INVALID_TARGET);
            return;
        }

        Monster monster = (Monster)effected;
        if (monster.isSpoiled())
        {
            effector.sendPacket(SystemMessageId.THE_TARGET_HAS_BEEN_ALREADY_ROBBED);
            return;
        }

        monster.setPlundered(player);

        if (!player.getInventory().checkInventorySlotsAndWeight(monster.getSpoilLootItems(), false, false))
            return;

        ICollection<ItemHolder>? items = monster.takeSweep();
        if (items != null)
        {
            foreach (ItemHolder sweepedItem in items)
            {
                ItemHolder rewardedItem = new ItemHolder(sweepedItem.getId(), sweepedItem.getCount());
                Party? party = effector.getParty();
                if (party != null)
                    party.distributeItem(player, rewardedItem, true, monster);
                else
                    player.addItem("Plunder", rewardedItem, effected, true);
            }
        }

        monster.getAI().notifyEvent(CtrlEvent.EVT_ATTACKED, effector);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}