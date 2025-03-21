using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// An effect that blocks a debuff. Acts like DOTA's Linken Sphere.
/// </summary>
[HandlerStringKey("AbnormalShield")]
public sealed class AbnormalShield: AbstractEffect
{
    private readonly int _times;

    /// <summary>
    /// An effect that blocks a debuff. Acts like DOTA's Linken Sphere.
    /// </summary>
    public AbnormalShield(EffectParameterSet parameters)
    {
        _times = parameters.GetInt32(XmlSkillEffectParameterType.Times, -1);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.setAbnormalShieldBlocks(_times);
    }

    public override EffectFlags EffectFlags => EffectFlags.ABNORMAL_SHIELD;

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.setAbnormalShieldBlocks(int.MinValue);
    }

    public override EffectTypes EffectTypes => EffectTypes.ABNORMAL_SHIELD;

    public override int GetHashCode() => _times;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._times);
}