using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Resurrection effect implementation.
/// </summary>
[HandlerStringKey("Resurrection")]
public sealed class Resurrection: AbstractEffect
{
    private readonly int _power;
    private readonly int _hpPercent;
    private readonly int _mpPercent;
    private readonly int _cpPercent;

    public Resurrection(EffectParameterSet parameters)
    {
        _power = parameters.GetInt32(XmlSkillEffectParameterType.Power, 0);
        _hpPercent = parameters.GetInt32(XmlSkillEffectParameterType.HpPercent, 0);
        _mpPercent = parameters.GetInt32(XmlSkillEffectParameterType.MpPercent, 0);
        _cpPercent = parameters.GetInt32(XmlSkillEffectParameterType.CpPercent, 0);
    }

    public override EffectTypes EffectTypes => EffectTypes.RESURRECTION;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
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