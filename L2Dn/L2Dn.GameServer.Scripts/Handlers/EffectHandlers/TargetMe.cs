using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Target Me effect implementation.
/// </summary>
[AbstractEffectName("TargetMe")]
public sealed class TargetMe: AbstractEffect
{
    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        if (effected.isPlayable())
            ((Playable)effected).setLockedTarget(null);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isPlayable())
        {
            if (effected.getTarget() != effector)
                effected.setTarget(effector);

            ((Playable)effected).setLockedTarget(effector);
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}