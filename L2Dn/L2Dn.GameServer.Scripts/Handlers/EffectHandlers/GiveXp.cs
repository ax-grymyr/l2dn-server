using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Give XP effect implementation.
/// </summary>
[HandlerStringKey("GiveXp")]
public sealed class GiveXp: AbstractEffect
{
    private readonly long _xp;
    private readonly int _level;
    private readonly double _percentage;

    public GiveXp(EffectParameterSet parameters)
    {
        _xp = parameters.GetInt64(XmlSkillEffectParameterType.Xp, 0);
        _level = parameters.GetInt32(XmlSkillEffectParameterType.Level, 0);
        _percentage = parameters.GetDouble(XmlSkillEffectParameterType.Percentage, 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (!effector.isPlayer() || player == null || !effected.isPlayer() || effected.isAlikeDead())
            return;

        double amount;
        if (player.getLevel() < _level)
            amount = _xp / 100.0 * _percentage;
        else
            amount = _xp;

        player.addExpAndSp(amount, 0);
    }

    public override int GetHashCode() => HashCode.Combine(_xp, _level, _percentage);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._xp, x._level, x._percentage));
}