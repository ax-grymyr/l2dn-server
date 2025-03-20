using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Item Effect: Increase/decrease PK count permanently.
/// </summary>
public sealed class PkCount: AbstractEffect
{
    private readonly int _amount;

    public PkCount(StatSet @params)
    {
        _amount = @params.getInt("amount", 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        if (player.getPkKills() > 0)
        {
            int newPkCount = Math.Max(player.getPkKills() + _amount, 0);
            player.setPkKills(newPkCount);
            player.updateUserInfo();
        }
    }

    public override int GetHashCode() => _amount;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._amount);
}