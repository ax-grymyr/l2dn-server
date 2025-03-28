using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Protection Blessing effect implementation.
/// </summary>
public sealed class ProtectionBlessing: AbstractEffect
{
    public ProtectionBlessing(StatSet @params)
    {
    }

    public override bool canStart(Creature effector, Creature effected, Skill skill)
    {
        return effector != null && effected != null && effected.isPlayer();
    }

    public override long getEffectFlags() => EffectFlag.PROTECTION_BLESSING.getMask();

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}