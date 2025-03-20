using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Take Fort effect implementation.
/// </summary>
[HandlerName("TakeFort")]
public sealed class TakeFort: AbstractEffect
{
    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (!effector.isPlayer() || player == null)
            return;

        Clan? clan = player.getClan();
        if (clan == null)
            return;

        Fort? fort = FortManager.getInstance().getFort(effector);
        if (fort != null && fort.getResidenceId() == FortManager.ORC_FORTRESS)
        {
            if (fort.getSiege().isInProgress())
            {
                fort.endOfSiege(clan);
                if (effector.isPlayer())
                {
                    FortSiegeManager.getInstance().dropCombatFlag(player, FortManager.ORC_FORTRESS);

                    Message mail = new Message(player.ObjectId, "Orc Fortress", "", MailType.NPC);
                    Mail attachment = mail.createAttachments();
                    attachment.addItem("Orc Fortress", Inventory.ADENA_ID, 30_000_000, player, player);
                    MailManager.getInstance().sendMessage(mail);
                }
            }
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}