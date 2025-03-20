using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Open Chest effect implementation.
/// </summary>
public sealed class OpenChest: AbstractEffect
{
    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected is not Chest chest)
            return;

        Player? player = effector.getActingPlayer();
        if (player == null)
            return;

        if (chest.isDead() || player.getInstanceWorld() != chest.getInstanceWorld())
            return;

        if ((player.getLevel() <= 77 && Math.Abs(chest.getLevel() - player.getLevel()) <= 6) ||
            (player.getLevel() >= 78 && Math.Abs(chest.getLevel() - player.getLevel()) <= 5))
        {
            player.broadcastSocialAction(3);
            chest.setSpecialDrop();
            chest.setMustRewardExpSp(false);
            chest.reduceCurrentHp(chest.getMaxHp(), player, skill);
        }
        else
        {
            player.broadcastSocialAction(13);
            chest.addDamageHate(player, 0, 1);
            chest.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, player);
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}