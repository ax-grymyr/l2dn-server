using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class GivePetXp: AbstractEffect
{
    private readonly int _xp;

    public GivePetXp(StatSet @params)
    {
        _xp = @params.getInt("xp", 0);
    }

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effector.hasPet())
            return;

        effected.getActingPlayer()?.getPet()?.addExpAndSp(_xp, 0);
    }

    public override int GetHashCode() => _xp;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._xp);
}