using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.AttackableTasks;

public class OnKillNotifyTask: Runnable
{
    private readonly Attackable _attackable;
    private readonly Quest _quest;
    private readonly Player _killer;
    private readonly bool _isSummon;

    public OnKillNotifyTask(Attackable attackable, Quest quest, Player killer, bool isSummon)
    {
        _attackable = attackable;
        _quest = quest;
        _killer = killer;
        _isSummon = isSummon;
    }

    public override void run()
    {
        if ((_quest != null) && (_attackable != null) && (_killer != null))
        {
            _quest.notifyKill(_attackable, _killer, _isSummon);
        }
    }
}