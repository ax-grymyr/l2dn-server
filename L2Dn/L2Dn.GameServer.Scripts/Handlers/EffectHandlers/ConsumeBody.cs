using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Consume Body effect implementation.
/// </summary>
public sealed class ConsumeBody: AbstractEffect
{
    public ConsumeBody(StatSet @params)
    {
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effected.isDead() //
            || effector.getTarget() != effected //
            || (!effected.isNpc() && !effected.isSummon()) //
            || (effected.isSummon() && effector != effected.getActingPlayer()))
        {
            return;
        }

        if (effected.isNpc())
            ((Npc)effected).endDecayTask();
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}