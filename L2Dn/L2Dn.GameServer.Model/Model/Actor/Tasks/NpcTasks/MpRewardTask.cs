using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Tasks.NpcTasks;

public class MpRewardTask
{
    private int _count;
    private readonly double _value;
    private readonly ScheduledFuture _task;
    private readonly Creature _creature;

    public MpRewardTask(Creature creature, Npc npc)
    {
        NpcTemplate template = npc.getTemplate();
        _creature = creature;
        _count = template.getMpRewardTicks();
        _value = calculateBaseValue(npc, creature);
        _task = ThreadPool.scheduleAtFixedRate(run, TimeSpan.FromMilliseconds(Config.EFFECT_TICK_RATIO),
            TimeSpan.FromMilliseconds(Config.EFFECT_TICK_RATIO));
    }

    /**
     * @param npc
     * @param creature
     * @return
     */
    private double calculateBaseValue(Npc npc, Creature creature)
    {
        NpcTemplate template = npc.getTemplate();
        switch (template.getMpRewardType())
        {
            case MpRewardType.PER:
            {
                return creature.getMaxMp() * (template.getMpRewardValue() / 100.0) / template.getMpRewardTicks();
            }
        }
        return 1.0 * template.getMpRewardValue() / template.getMpRewardTicks();
    }

    private void run()
    {
        Player? player = _creature.getActingPlayer();
        if (--_count <= 0 || (_creature.isPlayer() && player != null && !player.isOnline()))
        {
            _task.cancel(false);
            return;
        }

        _creature.setCurrentMp(_creature.getCurrentMp() + _value);
    }
}