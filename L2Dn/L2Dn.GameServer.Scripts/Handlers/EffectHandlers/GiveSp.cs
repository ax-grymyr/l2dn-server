using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Give SP effect implementation.
/// </summary>
public sealed class GiveSp: AbstractEffect
{
    private readonly int _sp;

    public GiveSp(StatSet @params)
    {
        _sp = @params.getInt("sp", 0);
    }

    public override bool IsInstant => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effector.isPlayer() || !effected.isPlayer() || effected.isAlikeDead())
            return;

        effector.getActingPlayer()?.addExpAndSp(0, _sp);
    }

    public override int GetHashCode() => _sp;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._sp);
}