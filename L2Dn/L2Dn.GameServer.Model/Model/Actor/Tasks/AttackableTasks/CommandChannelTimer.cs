using L2Dn.GameServer.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Tasks.AttackableTasks;

public class CommandChannelTimer: Runnable
{
    private readonly Attackable _attackable;

    public CommandChannelTimer(Attackable attackable)
    {
        _attackable = attackable;
    }

    public void run()
    {
        if (_attackable == null)
        {
            return;
        }

        if (DateTime.UtcNow - _attackable.getCommandChannelLastAttack() >
            TimeSpan.FromMilliseconds(Config.Character.LOOT_RAIDS_PRIVILEGE_INTERVAL))
        {
            _attackable.setCommandChannelTimer(null);
            _attackable.setFirstCommandChannelAttacked(null);
            _attackable.setCommandChannelLastAttack(null);
        }
        else
        {
            ThreadPool.schedule(this, 10000); // 10sec
        }
    }
}