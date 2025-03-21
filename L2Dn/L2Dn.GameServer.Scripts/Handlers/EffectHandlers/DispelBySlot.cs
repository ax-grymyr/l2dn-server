using System.Collections.Frozen;
using System.Globalization;
using L2Dn.Extensions;
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
/// Dispel By Slot effect implementation.
/// </summary>
[HandlerStringKey("DispelBySlot")]
public sealed class DispelBySlot: AbstractEffect
{
    private readonly FrozenDictionary<AbnormalType, short> _dispelAbnormals;

    public DispelBySlot(EffectParameterSet parameters)
    {
        _dispelAbnormals = FrozenDictionary<AbnormalType, short>.Empty;
        string dispel = parameters.GetString(XmlSkillEffectParameterType.Dispel);
        if (!string.IsNullOrEmpty(dispel))
        {
            _dispelAbnormals = dispel.Split(";").Select(ngtStack =>
            {
                string[] ngt = ngtStack.Split(",");
                return new KeyValuePair<AbnormalType, short>(Enum.Parse<AbnormalType>(ngt[0], true),
                    short.Parse(ngt[1], CultureInfo.InvariantCulture));
            }).ToFrozenDictionary();
        }
    }

    public override EffectTypes EffectTypes => EffectTypes.DISPEL_BY_SLOT;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_dispelAbnormals.Count == 0)
            return;

        // Continue only if target has any of the abnormals. Save useless cycles.
        if (effected.getEffectList().hasAbnormalType(_dispelAbnormals.Keys))
        {
            // Dispel transformations (buff and by GM)
            if (_dispelAbnormals.TryGetValue(AbnormalType.TRANSFORM, out short transformToDispel) &&
                (transformToDispel == effected.getTransformationId() || transformToDispel < 0))
            {
                effected.stopTransformation(true);
            }

            effected.getEffectList().stopEffects(info =>
            {
                // We have already dealt with transformation from above.
                if (info.isAbnormalType(AbnormalType.TRANSFORM))
                {
                    return false;
                }

                return _dispelAbnormals.TryGetValue(info.getSkill().AbnormalType, out short abnormalLevel) &&
                    (abnormalLevel < 0 || abnormalLevel >= info.getSkill().AbnormalLevel);
            }, true, true);
        }
    }

    public override int GetHashCode() => _dispelAbnormals.GetDictionaryHashCode();

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => x._dispelAbnormals.GetDictionaryComparable());
}