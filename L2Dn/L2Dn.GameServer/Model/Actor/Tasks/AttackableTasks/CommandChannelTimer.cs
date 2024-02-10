using L2Dn.GameServer.Utilities;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Tasks.AttackableTasks;

public class CommandChannelTimer: Runnable
{
    private readonly Attackable _attackable;

    public CommandChannelTimer(Attackable attackable)
    {
        _attackable = attackable;
    }

    public override void run()
    {
        if (_attackable == null)
        {
            return;
        }

        if ((System.currentTimeMillis() - _attackable.getCommandChannelLastAttack()) >
            Config.LOOT_RAIDS_PRIVILEGE_INTERVAL)
        {
            _attackable.setCommandChannelTimer(null);
            _attackable.setFirstCommandChannelAttacked(null);
            _attackable.setCommandChannelLastAttack(0);
        }
        else
        {
            ThreadPool.schedule(this, 10000); // 10sec
        }
    }
}
