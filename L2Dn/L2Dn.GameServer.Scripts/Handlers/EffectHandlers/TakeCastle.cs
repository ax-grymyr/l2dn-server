using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Take Castle effect implementation.
/// </summary>
public sealed class TakeCastle: AbstractEffect
{
    private readonly CastleSide _side;

    public TakeCastle(StatSet @params)
    {
        _side = @params.getEnum<CastleSide>("side");
    }

    public override bool IsInstant => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effector.isPlayer())
            return;

        Clan? clan = effector.getClan();
        if (clan is null)
            return;

        Castle? castle = CastleManager.getInstance().getCastle(effector);
        if (castle is null)
            return;

        castle.engrave(clan, effected, _side);
    }

    public override int GetHashCode() => HashCode.Combine(_side);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._side);
}