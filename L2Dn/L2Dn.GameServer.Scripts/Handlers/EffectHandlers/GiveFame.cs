using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class GiveFame: AbstractEffect
{
    private readonly int _fame;

    public GiveFame(StatSet @params)
    {
        _fame = @params.getInt("fame", 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (!effector.isPlayer() || player == null || !effected.isPlayer() || effected.isAlikeDead())
            return;

        player.setFame(player.getFame() + _fame);
    }

    public override int GetHashCode() => _fame;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._fame);
}