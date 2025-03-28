using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Transfer Damage effect implementation.
/// </summary>
public sealed class TransferDamageToPlayer(StatSet @params)
    : AbstractStatAddEffect(@params, Stat.TRANSFER_DAMAGE_TO_PLAYER)
{
    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        if (effected.isPlayable() && effector.isPlayer())
            ((Playable)effected).setTransferDamageTo(null);
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (effected.isPlayable() && effector.isPlayer() && player != null)
            ((Playable)effected).setTransferDamageTo(player);
    }
}