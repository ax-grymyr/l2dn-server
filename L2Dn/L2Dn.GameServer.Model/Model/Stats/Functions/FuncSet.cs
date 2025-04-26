using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Stats.Functions;

/// <summary>
/// Returns the function value, if the condition are met.
/// </summary>
[HandlerKey<StatFuncType>(StatFuncType.SET)]
public sealed class FuncSet(StatFuncParameters parameters): AbstractFunction(parameters)
{
    public override double Calc(Creature effector, Creature effected, Skill skill, double initialValue)
    {
        if (ApplyCondition == null || ApplyCondition.test(effector, effected, skill))
            return Value;

        return initialValue;
    }
}