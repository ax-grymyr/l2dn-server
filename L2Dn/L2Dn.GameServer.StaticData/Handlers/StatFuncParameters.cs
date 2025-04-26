using System.Reflection;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Stats.Functions;

public sealed class StatFuncParameters(StatFuncType funcType, int order, Stat stat, double value,
    IConditionBase? applyCondition = null)
{
    public StatFuncType FuncType { get; } = funcType;

    /// <summary>
    /// The function stat.
    /// </summary>
    public Stat Stat { get; } = stat;

    /// <summary>
    /// The function priority order.
    /// </summary>
    public int Order { get; } = order >= 0 ? order : (int)funcType;

    /// <summary>
    /// The function value.
    /// </summary>
    public double Value { get; } = value;

    public IConditionBase? ApplyCondition { get; } = applyCondition;
}