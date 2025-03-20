using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using NLog;

namespace L2Dn.GameServer.Model.Stats.Functions;

/**
 * A Function object is a component of a Calculator created to manage and dynamically calculate the effect of a character property (ex : MAX_HP, REGENERATE_HP_RATE...).<br>
 * In fact, each calculator is a table of functions object in which each function represents a mathematics function:<br>
 * FuncAtkAccuracy -> Math.sqrt(_player.getDEX())*6+_player.getLevel()<br>
 * When the calc method of a calculator is launched, each mathematics function is called according to its priority <b>_order</b>.<br>
 * Indeed, functions with lowest priority order is executed first and functions with the same order are executed in unspecified order.<br>
 * @author Zoey76
 */
public abstract class AbstractFunction
{
	/** Logger. */
	protected static readonly Logger LOG = LogManager.GetLogger(nameof(AbstractFunction));

	/** Statistics, that is affected by this function (See Creature.CALCULATOR_XXX constants) */
	private readonly Stat _stat;

	/**
	 * Order of functions calculation.<br>
	 * Functions with lower order are executed first.<br>
	 * Functions with the same order are executed in unspecified order.<br>
	 * Usually add/subtract functions has lowest order,<br>
	 * then bonus/penalty functions (multiply/divide) are applied, then functions that do more complex<br>
	 * calculations (non-linear functions).
	 */
	private readonly int _order;

	/**
	 * Owner can be an armor, weapon, skill, system event, quest, etc.<br>
	 * Used to remove all functions added by this owner.
	 */
	private readonly object _funcOwner;

	/** Function may be disabled by attached condition. */
	private readonly Condition _applayCond;

	/** The value. */
	private readonly double _value;

	/**
	 * Constructor of Func.
	 * @param stat the stat
	 * @param order the order
	 * @param owner the owner
	 * @param value the value
	 * @param applayCond the apply condition
	 */
	public AbstractFunction(Stat stat, int order, object owner, double value, Condition applayCond)
	{
		_stat = stat;
		_order = order;
		_funcOwner = owner;
		_value = value;
		_applayCond = applayCond;
	}

	/**
	 * Gets the apply condition
	 * @return the apply condition
	 */
	public Condition getApplayCond()
	{
		return _applayCond;
	}

	/**
	 * Gets the fuction owner.
	 * @return the function owner
	 */
	public object getFuncOwner()
	{
		return _funcOwner;
	}

	/**
	 * Gets the function order.
	 * @return the order
	 */
	public int getOrder()
	{
		return _order;
	}

	/**
	 * Gets the stat.
	 * @return the stat
	 */
	public Stat getStat()
	{
		return _stat;
	}

	/**
	 * Gets the value.
	 * @return the value
	 */
	public double getValue()
	{
		return _value;
	}

	/**
	 * Run the mathematics function of the Func.
	 * @param effector the effector
	 * @param effected the effected
	 * @param skill the skill
	 * @param initVal the initial value
	 * @return the calculated value
	 */
	public abstract double calc(Creature effector, Creature effected, Skill skill, double initVal);
}