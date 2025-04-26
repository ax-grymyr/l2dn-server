using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Stats.Functions;

/// <summary>
/// A Function object is a component of a Calculator created to manage and dynamically calculate the effect of
/// a character property (ex : MAX_HP, REGENERATE_HP_RATE...). When the calc method of a calculator is launched,
/// each mathematics function is called according to its priority order.
/// </summary>
public abstract class AbstractFunction(StatFuncParameters parameters): IStatFuncBase
{
    private readonly Stat _stat = parameters.Stat;
    private readonly int _order = parameters.Order;
    private readonly double _value = parameters.Value;
    private readonly Condition? _applyCondition = (Condition?)parameters.ApplyCondition;

    /// <summary>
    /// Gets the apply condition.
    /// </summary>
    public Condition? ApplyCondition => _applyCondition;

    /// <summary>
    /// Gets the function order.
    /// </summary>
    public int Order => _order;

    /// <summary>
    /// Statistics, that is affected by this function.
    /// </summary>
    public Stat Stat => _stat;

    public double Value => _value;

    /// <summary>
    /// Run the mathematics function of the Func.
    /// </summary>
    public abstract double Calc(Creature effector, Creature effected, Skill skill, double initialValue);
}