using System.Collections.Concurrent;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public abstract class AbstractConditionalHpEffect: AbstractStatEffect
{
	private readonly int _hpPercent;
	private readonly ConcurrentDictionary<Creature, AtomicBoolean> _updates = new();

	protected AbstractConditionalHpEffect(StatSet @params, Stat stat): base(@params, stat)
	{
		_hpPercent = @params.getInt("hpPercent", 0);
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
	{
		// Augmentation option
		if (skill is null)
			return;

		// Register listeners
		if (_hpPercent > 0 && _updates.ContainsKey(effected))
		{
			if (_updates.TryAdd(effected, new AtomicBoolean(canPump(effector, effected, skill))))
				effected.Events.Subscribe<OnCreatureHpChange>(this, OnCreatureHpChanged);
		}
	}

	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		// Augmentation option
		if (skill == null)
			return;

		effected.Events.Unsubscribe<OnCreatureHpChange>(OnCreatureHpChanged);
		_updates.TryRemove(effected, out _);
	}

	public override bool canPump(Creature? effector, Creature effected, Skill? skill)
	{
		return _hpPercent <= 0 || effected.getCurrentHpPercent() <= _hpPercent;
	}

	private void OnCreatureHpChanged(OnCreatureHpChange @event)
	{
		Creature creature = @event.getCreature();
		if (!_updates.TryGetValue(creature, out AtomicBoolean? update))
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