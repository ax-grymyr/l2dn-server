using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Stats.Functions;

/// <summary>
/// Returns the initial value plus the function value, if the condition are met.
/// </summary>
[HandlerKey<StatFuncType>(StatFuncType.ADD)]
public sealed class FuncAdd(StatFuncParameters parameters): AbstractFunction(parameters)
{
    public override double Calc(Creature effector, Creature effected, Skill skill, double initialValue)
    {
        if (ApplyCondition == null || ApplyCondition.test(effector, effected, skill))
            return initialValue + Value;

        return initialValue;
    }
}