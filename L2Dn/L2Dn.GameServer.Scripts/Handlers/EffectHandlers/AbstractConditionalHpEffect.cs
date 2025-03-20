using System.Collections.Concurrent;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public abstract class AbstractConditionalHpEffect: AbstractStatEffect
{
    private readonly ConcurrentDictionary<Creature, AtomicBoolean> _updates = new();
    private readonly int _hpPercent;

    protected AbstractConditionalHpEffect(EffectParameterSet parameters, Stat stat): base(parameters, stat)
    {
        _hpPercent = parameters.GetInt32(XmlSkillEffectParameterType.HpPercent, 0);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        // Augmentation option
        if (skill is null)
            return;

        // Register listeners
        if (_hpPercent > 0 && _updates.ContainsKey(effected))
        {
            if (_updates.TryAdd(effected, new AtomicBoolean(CanPump(effector, effected, skill))))
                effected.Events.Subscribe<OnCreatureHpChange>(this, OnCreatureHpChanged);
        }
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        // Augmentation option
        if (skill is null)
            return;

        effected.Events.Unsubscribe<OnCreatureHpChange>(OnCreatureHpChanged);
        _updates.TryRemove(effected, out _);
    }

    public override bool CanPump(Creature? effector, Creature effected, Skill? skill)
    {
        return _hpPercent <= 0 || effected.getCurrentHpPercent() <= _hpPercent;
    }

    private void OnCreatureHpChanged(OnCreatureHpChange @event)
    {
        Creature creature = @event.getCreature();
        if (!_updates.TryGetValue(creature, out AtomicBoolean? update))
            return;

        if (CanPump(null, creature, null))
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

    public override int GetHashCode() => _hpPercent;
    public override bool Equals(object? obj) => base.Equals(obj) && this.EqualsTo(obj, static x => x._hpPercent);
}