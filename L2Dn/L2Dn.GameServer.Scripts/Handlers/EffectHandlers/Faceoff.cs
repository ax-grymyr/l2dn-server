using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("Faceoff")]
public sealed class Faceoff: AbstractEffect
{
    public override EffectFlags EffectFlags => EffectFlags.FACEOFF;

    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        return effected.isPlayer();
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}