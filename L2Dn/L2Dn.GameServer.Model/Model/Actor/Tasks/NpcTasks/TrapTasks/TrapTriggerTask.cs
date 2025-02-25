using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Tasks.NpcTasks.TrapTasks;

/**
 * Trap trigger task.
 * @author Zoey76
 */
public sealed class TrapTriggerTask(Trap trap): Runnable
{
    public void run()
    {
        try
        {
            Skill? trapSkill = trap.getSkill();
            trap.doCast(trapSkill);
            ThreadPool.schedule(new TrapUnsummonTask(trap), trapSkill.getHitTime() + TimeSpan.FromMilliseconds(300));
        }
        catch (Exception e)
        {
            trap.unSummon();
        }
    }
}