using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Resurrection Special effect implementation.
/// </summary>
public sealed class ResurrectionSpecial: AbstractEffect
{
    private readonly int _power;
    private readonly int _hpPercent;
    private readonly int _mpPercent;
    private readonly int _cpPercent;
    private readonly FrozenSet<int> _instanceIds;

    public ResurrectionSpecial(EffectParameterSet parameters)
    {
        _power = parameters.GetInt32(XmlSkillEffectParameterType.Power, 0);
        _hpPercent = parameters.GetInt32(XmlSkillEffectParameterType.HpPercent, 0);
        _mpPercent = parameters.GetInt32(XmlSkillEffectParameterType.MpPercent, 0);
        _cpPercent = parameters.GetInt32(XmlSkillEffectParameterType.CpPercent, 0);

        string instanceIds = parameters.GetString(XmlSkillEffectParameterType.InstanceId, string.Empty);
        _instanceIds = ParseUtil.ParseSet<int>(instanceIds);
    }

    public override EffectTypes EffectTypes => EffectTypes.RESURRECTION_SPECIAL;

    public override EffectFlags EffectFlags => EffectFlags.RESURRECTION_SPECIAL;

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        Player? effectedPlayer = effected.getActingPlayer();
        if (!effected.isPlayer() && !effected.isPet() || effectedPlayer == null)
            return;

        if (effectedPlayer.isInOlympiadMode())
            return;

        Player? caster = effector.getActingPlayer();
        Instance? instance = caster?.getInstanceWorld();
        if (_instanceIds.Count != 0 && (instance == null || !_instanceIds.Contains(instance.getTemplateId())))
            return;

        if (effected.isPlayer() && caster != null)
        {
            effectedPlayer.reviveRequest(caster, false, _power, _hpPercent, _mpPercent, _cpPercent);
        }
        else if (effected.isPet())
        {
            Pet pet = (Pet)effected;
            effectedPlayer.reviveRequest(pet.getActingPlayer(), true, _power, _hpPercent, _mpPercent, _cpPercent);
        }
    }

    public override int GetHashCode() =>
        HashCode.Combine(_power, _hpPercent, _mpPercent, _cpPercent, _instanceIds.GetSetHashCode());

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj,
            static x => (x._power, x._hpPercent, x._mpPercent, x._cpPercent, x._instanceIds.GetSetComparable()));
}