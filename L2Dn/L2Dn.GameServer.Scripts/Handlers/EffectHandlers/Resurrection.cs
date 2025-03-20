using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.TaskManagers;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Resurrection effect implementation.
/// </summary>
public sealed class Resurrection: AbstractEffect
{
    private readonly int _power;
    private readonly int _hpPercent;
    private readonly int _mpPercent;
    private readonly int _cpPercent;

    public Resurrection(StatSet @params)
    {
        _power = @params.getInt("power", 0);
        _hpPercent = @params.getInt("hpPercent", 0);
        _mpPercent = @params.getInt("mpPercent", 0);
        _cpPercent = @params.getInt("cpPercent", 0);
    }

    public override EffectTypes EffectType => EffectTypes.RESURRECTION;

    public override bool IsInstant => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        Player? effectedPlayer = effected.getActingPlayer();

        if (effector.isPlayer() && player != null)
        {
            if (effectedPlayer == null)
                return;

            if (!effectedPlayer.isResurrectionBlocked() && !effectedPlayer.isReviveRequested())
                effectedPlayer.reviveRequest(player, effected.isPet(), _power, _hpPercent, _mpPercent, _cpPercent);
        }
        else
        {
            DecayTaskManager.getInstance().cancel(effected);
            effected.doRevive(Formulas.calculateSkillResurrectRestorePercent(_power, effector));
        }
    }

    public override int GetHashCode() => HashCode.Combine(_power, _hpPercent, _mpPercent, _cpPercent);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._power, x._hpPercent, x._mpPercent, x._cpPercent));
}