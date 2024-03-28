using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Instances;

/**
 * Class for Control Tower instance.
 */
public class ControlTower: Tower
{
    private Set<Spawn> _guards;

    public ControlTower(NpcTemplate template): base(template)
    {
        setInstanceType(InstanceType.ControlTower);
    }

    public override bool doDie(Creature killer)
    {
        if (getCastle().getSiege().isInProgress())
        {
            getCastle().getSiege().killedCT(this);

            if ((_guards != null) && !_guards.isEmpty())
            {
                foreach (Spawn spawn in _guards)
                {
                    if (spawn == null)
                    {
                        continue;
                    }

                    try
                    {
                        spawn.stopRespawn();
                        // spawn.getLastSpawn().doDie(spawn.getLastSpawn());
                    }
                    catch (Exception e)
                    {
                        LOGGER.Warn("Error at ControlTower: " + e);
                    }
                }

                _guards.Clear();
            }
        }

        return base.doDie(killer);
    }

    public void registerGuard(Spawn guard)
    {
        getGuards().add(guard);
    }

    private Set<Spawn> getGuards()
    {
        if (_guards == null)
        {
            lock (this)
            {
                if (_guards == null)
                {
                    _guards = new();
                }
            }
        }

        return _guards;
    }
}
