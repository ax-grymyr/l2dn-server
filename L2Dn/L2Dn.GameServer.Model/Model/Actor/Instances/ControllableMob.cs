using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class ControllableMob: Monster
{
    private bool _isInvul;

    public override bool isAggressive()
    {
        return true;
    }

    public override int getAggroRange()
    {
        // force mobs to be aggro
        return 500;
    }

    public ControllableMob(NpcTemplate template): base(template)
    {
        InstanceType = InstanceType.ControllableMob;
    }

    protected override CreatureAI initAI()
    {
        return new ControllableMobAI(this);
    }

    public override void detachAI()
    {
        // do nothing, AI of controllable mobs can't be detached automatically
    }

    public override bool isInvul()
    {
        return _isInvul;
    }

    public override void setInvul(bool isInvul)
    {
        _isInvul = isInvul;
    }

    public override bool doDie(Creature killer)
    {
        if (!base.doDie(killer))
        {
            return false;
        }

        setAI(null);
        return true;
    }
}