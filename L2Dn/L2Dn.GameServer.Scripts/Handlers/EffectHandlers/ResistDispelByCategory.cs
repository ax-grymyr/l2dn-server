using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class ResistDispelByCategory: AbstractEffect
{
    private readonly DispelSlotType _slot;
    private readonly double _amount;

    public ResistDispelByCategory(EffectParameterSet parameters)
    {
        _amount = parameters.GetDouble(XmlSkillEffectParameterType.Amount, 0);
        _slot = parameters.GetEnum(XmlSkillEffectParameterType.Slot, DispelSlotType.BUFF);
    }

    public override void Pump(Creature effected, Skill skill)
    {
        switch (_slot)
        {
            // Only this one is in use it seems
            case DispelSlotType.BUFF:
            {
                effected.getStat().mergeMul(Stat.RESIST_DISPEL_BUFF, 1 + _amount / 100);
                break;
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(_slot, _amount);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._slot, x._amount));
}