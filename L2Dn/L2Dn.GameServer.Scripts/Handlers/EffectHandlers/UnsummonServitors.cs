using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Unsummon my servitors effect implementation.
/// </summary>
[AbstractEffectName("UnsummonServitors")]
public sealed class UnsummonServitors: AbstractEffect
{
    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effector.hasServitors())
            return;

        effector.getServitors().Values.ForEach(servitor =>
        {
            servitor.abortAttack();
            servitor.abortCast();
            servitor.stopAllEffects();

            Player? player = effector.getActingPlayer();
            servitor.unSummon(player);
        });
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}