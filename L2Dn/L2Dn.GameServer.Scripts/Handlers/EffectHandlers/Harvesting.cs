using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Harvesting effect implementation.
/// </summary>
public sealed class Harvesting: AbstractEffect
{
    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (!effector.isPlayer() || player == null || !effected.isMonster() || !effected.isDead())
            return;

        Monster monster = (Monster)effected;
        if (player.ObjectId != monster.getSeederId())
        {
            player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_HARVEST);
        }
        else if (monster.isSeeded())
        {
            if (calcSuccess(player, monster))
            {
                ItemHolder? harvestedItem = monster.takeHarvest();
                if (harvestedItem != null)
                {
                    // Add item
                    player.getInventory().addItem("Harvesting", harvestedItem.Id, harvestedItem.getCount(), player,
                        monster);

                    // Send system msg
                    SystemMessagePacket sm;
                    if (harvestedItem.getCount() == 1)
                    {
                        sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1);
                        sm.Params.addItemName(harvestedItem.Id);
                    }
                    else
                    {
                        sm = new SystemMessagePacket(SystemMessageId.YOU_VE_OBTAINED_S1_X_S2);
                        sm.Params.addItemName(harvestedItem.Id);
                        sm.Params.addLong(harvestedItem.getCount());
                    }

                    player.sendPacket(sm);

                    // Send msg to party
                    Party? party = player.getParty();
                    if (party != null)
                    {
                        if (harvestedItem.getCount() == 1)
                        {
                            sm = new SystemMessagePacket(SystemMessageId.C1_HAS_OBTAINED_S2_2);
                            sm.Params.addString(player.getName());
                            sm.Params.addItemName(harvestedItem.Id);
                        }
                        else
                        {
                            sm = new SystemMessagePacket(SystemMessageId.C1_HARVESTED_S3_S2_S);
                            sm.Params.addString(player.getName());
                            sm.Params.addLong(harvestedItem.getCount());
                            sm.Params.addItemName(harvestedItem.Id);
                        }

                        party.broadcastToPartyMembers(player, sm);
                    }
                }
            }
            else
            {
                player.sendPacket(SystemMessageId.THE_HARVEST_HAS_FAILED);
            }
        }
        else
        {
            player.sendPacket(SystemMessageId.THE_HARVEST_FAILED_BECAUSE_THE_SEED_WAS_NOT_SOWN);
        }
    }

    private static bool calcSuccess(Player player, Monster target)
    {
        int levelPlayer = player.getLevel();
        int levelTarget = target.getLevel();

        int diff = levelPlayer - levelTarget;
        if (diff < 0)
            diff = -diff;

        // apply penalty, target <=> player levels
        // 5% penalty for each level
        int basicSuccess = 100;
        if (diff > 5)
            basicSuccess -= (diff - 5) * 5;

        // success rate can't be less than 1%
        if (basicSuccess < 1)
            basicSuccess = 1;

        return Rnd.get(99) < basicSuccess;
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}