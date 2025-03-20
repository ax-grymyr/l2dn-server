using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Transfer Damage effect implementation.
/// </summary>
public sealed class TransferDamageToPlayer(EffectParameterSet parameters)
    : AbstractStatAddEffect(parameters, Stat.TRANSFER_DAMAGE_TO_PLAYER)
{
    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        if (effected.isPlayable() && effector.isPlayer())
            ((Playable)effected).setTransferDamageTo(null);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (effected.isPlayable() && effector.isPlayer() && player != null)
            ((Playable)effected).setTransferDamageTo(player);
    }
}