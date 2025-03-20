using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class SayhaGraceSupport: AbstractEffect
{
    public SayhaGraceSupport(StatSet @params)
    {
    }

    public override bool canStart(Creature effector, Creature effected, Skill skill)
    {
        return effected != null && effected.isPlayer();
    }

    public override bool IsInstant => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (player is null)
            return;

        double rnd = Rnd.nextDouble() * 100;
        if (rnd <= 0.1) // 4h
            player.setSayhaGraceSupportEndTime(DateTime.UtcNow + TimeSpan.FromMilliseconds(3600000 * 4));
        else if (rnd <= 0.3) // 3h
            player.setSayhaGraceSupportEndTime(DateTime.UtcNow + TimeSpan.FromMilliseconds(3600000 * 3));
        else if (rnd <= 0.6) // 2h
            player.setSayhaGraceSupportEndTime(DateTime.UtcNow + TimeSpan.FromMilliseconds(3600000 * 2));
        else if (rnd <= 1.1) // 1h
            player.setSayhaGraceSupportEndTime(DateTime.UtcNow + TimeSpan.FromMilliseconds(3600000 * 1));
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}