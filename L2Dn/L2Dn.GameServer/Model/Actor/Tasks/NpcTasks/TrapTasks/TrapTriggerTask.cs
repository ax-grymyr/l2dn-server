using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Utilities;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Tasks.NpcTasks.TrapTasks;

/**
 * Trap trigger task.
 * @author Zoey76
 */
public class TrapTriggerTask: Runnable
{
    private readonly Trap _trap;

    public TrapTriggerTask(Trap trap)
    {
        _trap = trap;
    }

    public override void run()
    {
        try
        {
            _trap.doCast(_trap.getSkill());
            ThreadPool.schedule(new TrapUnsummonTask(_trap), _trap.getSkill().getHitTime() + 300);
        }
        catch (Exception e)
        {
            _trap.unSummon();
        }
    }
}