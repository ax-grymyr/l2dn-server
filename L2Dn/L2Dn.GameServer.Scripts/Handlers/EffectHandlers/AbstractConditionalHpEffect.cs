using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public abstract class AbstractConditionalHpEffect: AbstractStatEffect
{
	private readonly int _hpPercent;
	private readonly Map<Creature, AtomicBoolean> _updates = new();
	
	protected AbstractConditionalHpEffect(StatSet @params, Stat stat): base(@params, stat)
	{
		_hpPercent = @params.getInt("hpPercent", 0);
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		// Augmentation option
		if (skill == null)
		{
			return;
		}
		
		// Register listeners
		if ((_hpPercent > 0) && !_updates.containsKey(effected))
		{
			_updates.put(effected, new AtomicBoolean(canPump(effector, effected, skill)));
			effected.Events.Subscribe<OnCreatureHpChange>(this, onHpChange);
		}
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		// Augmentation option
		if (skill == null)
		{
			return;
		}
		
		effected.Events.Unsubscribe<OnCreatureHpChange>(onHpChange);
		_updates.remove(effected);
	}
	
	public override bool canPump(Creature effector, Creature effected, Skill skill)
	{
		return (_hpPercent <= 0) || (effected.getCurrentHpPercent() <= _hpPercent);
	}
	
	private void onHpChange(OnCreatureHpChange @event)
	{
		Creature creature = @event.getCreature();
		AtomicBoolean update = _updates.get(creature);
		if (update == null)
		{
			return;
		}
		if (canPump(null, creature, null))
		{
			if (update.get())
			{
				update.set(false);
				ThreadPool.execute(() => creature.getStat().recalculateStats(true));
			}
		}
		else if (!update.get())
		{
			update.set(true);
			ThreadPool.execute(() => creature.getStat().recalculateStats(true));
		}
	}
}