using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("ResistAbnormalByCategory")]
public sealed class ResistAbnormalByCategory: AbstractEffect
{
    private readonly DispelSlotType _slot;
    private readonly double _amount;

    public ResistAbnormalByCategory(EffectParameterSet parameters)
    {
        _amount = parameters.GetDouble(XmlSkillEffectParameterType.Amount, 0);
        _slot = parameters.GetEnum(XmlSkillEffectParameterType.Slot, DispelSlotType.DEBUFF);
    }

    public override void Pump(Creature effected, Skill skill)
    {
        switch (_slot)
        {
            // Only this one is in use it seems
            case DispelSlotType.DEBUFF:
            {
                effected.getStat().mergeMul(Stat.RESIST_ABNORMAL_DEBUFF, 1 + _amount / 100);
                break;
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(_slot, _amount);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._slot, x._amount));
}