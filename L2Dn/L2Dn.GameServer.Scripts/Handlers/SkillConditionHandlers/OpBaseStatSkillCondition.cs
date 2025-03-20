using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

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
        int currentValue = 0;
        switch (_stat)
        {
            case BaseStat.STR:
            {
                currentValue = caster.getSTR();
                break;
            }
            case BaseStat.INT:
            {
                currentValue = caster.getINT();
                break;
            }
            case BaseStat.DEX:
            {
                currentValue = caster.getDEX();
                break;
            }
            case BaseStat.WIT:
            {
                currentValue = caster.getWIT();
                break;
            }
            case BaseStat.CON:
            {
                currentValue = caster.getCON();
                break;
            }
            case BaseStat.MEN:
            {
                currentValue = caster.getMEN();
                break;
            }
        }

        return currentValue >= _min && currentValue <= _max;
    }
}