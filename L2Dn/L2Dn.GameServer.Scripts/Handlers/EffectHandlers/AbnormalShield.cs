using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// An effect that blocks a debuff. Acts like DOTA's Linken Sphere.
/// </summary>
public sealed class AbnormalShield: AbstractEffect
{
    private readonly int _times;

    /// <summary>
    /// An effect that blocks a debuff. Acts like DOTA's Linken Sphere.
    /// </summary>
    public AbnormalShield(StatSet @params)
    {
        _times = @params.getInt("times", -1);
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.setAbnormalShieldBlocks(_times);
    }

    public override EffectFlags EffectFlags => EffectFlags.ABNORMAL_SHIELD;

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        effected.setAbnormalShieldBlocks(int.MinValue);
    }

    public override EffectTypes EffectType => EffectTypes.ABNORMAL_SHIELD;

    public override int GetHashCode() => _times;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._times);
}