using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpBaseStat")]
public sealed class OpBaseStatSkillCondition: ISkillCondition
{
    private readonly BaseStat _stat;
    private readonly int _min;
    private readonly int _max;

    public OpBaseStatSkillCondition(SkillConditionParameterSet parameters)
    {
        _stat = parameters.GetEnum<BaseStat>(XmlSkillConditionParameterType.Stat);
        _min = parameters.GetInt32(XmlSkillConditionParameterType.Min, 0);
        _max = parameters.GetInt32(XmlSkillConditionParameterType.Max, int.MaxValue);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        int currentValue = _stat switch
        {
            BaseStat.STR => caster.getSTR(),
            BaseStat.INT => caster.getINT(),
            BaseStat.DEX => caster.getDEX(),
            BaseStat.WIT => caster.getWIT(),
            BaseStat.CON => caster.getCON(),
            BaseStat.MEN => caster.getMEN(),
            _ => 0,
        };

        return currentValue >= _min && currentValue <= _max;
    }

    public override int GetHashCode() => HashCode.Combine(_stat, _min, _max);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._stat, x._min, x._max));
}