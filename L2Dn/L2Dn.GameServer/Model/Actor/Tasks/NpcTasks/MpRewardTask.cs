using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;

namespace L2Dn.GameServer.Model.Actor.Tasks.NpcTasks;

public class MpRewardTask
{
    private int _count;
    private readonly double _value;
    private readonly ScheduledFuture<?> _task;
    private readonly Creature _creature;
	
    public MpRewardTask(Creature creature, Npc npc)
    {
        NpcTemplate template = npc.getTemplate();
        _creature = creature;
        _count = template.getMpRewardTicks();
        _value = calculateBaseValue(npc, creature);
        _task = ThreadPool.scheduleAtFixedRate(this::run, Config.EFFECT_TICK_RATIO, Config.EFFECT_TICK_RATIO);
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
                return (creature.getMaxMp() * (template.getMpRewardValue() / 100d)) / template.getMpRewardTicks();
            }
        }
        return template.getMpRewardValue() / template.getMpRewardTicks();
    }
	
    private void run()
    {
        if ((--_count <= 0) || (_creature.isPlayer() && !_creature.getActingPlayer().isOnline()))
        {
            _task.cancel(false);
            return;
        }
		
        _creature.setCurrentMp(_creature.getCurrentMp() + _value);
    }
}
