using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Templates;
using NLog;

namespace L2Dn.GameServer.Model.Stats.Functions;

/// <summary>
/// Returns the initial value divided the function value, if the condition are met.
/// </summary>
[HandlerKey<StatFuncType>(StatFuncType.DIV)]
public sealed class FuncDiv(StatFuncParameters parameters): AbstractFunction(parameters)
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(FuncDiv));

    public override double Calc(Creature effector, Creature effected, Skill skill, double initialValue)
    {
        if (ApplyCondition == null || ApplyCondition.test(effector, effected, skill))
        {
            try
            {
                return initialValue / Value;
            }
            catch (Exception e)
            {
                _logger.Warn(nameof(FuncDiv) + ": Division by zero: " + Value + "! " + e);
            }
        }

        return initialValue;
    }
}