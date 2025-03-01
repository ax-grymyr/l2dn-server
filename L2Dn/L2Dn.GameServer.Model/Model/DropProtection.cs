using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model;

public class DropProtection: Runnable
{
    private volatile bool _isProtected;
    private Creature? _owner;
    private ScheduledFuture? _task;

    private const long PROTECTED_MILLIS_TIME = 15000;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void run()
    {
        _isProtected = false;
        _owner = null;
        _task = null;
    }

    public bool isProtected()
    {
        return _isProtected;
    }

    public Creature? getOwner()
    {
        return _owner;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool tryPickUp(Player actor)
    {
        if (!_isProtected)
        {
            return true;
        }

        if (_owner == actor)
        {
            return true;
        }

        Party? ownerParty = _owner?.getParty();
        if (ownerParty != null && ownerParty == actor.getParty())
            return true;

        /*
         * if (_owner.getClan() != null && _owner.getClan() == actor.getClan()) return true;
         */

        return false;
    }

    public bool tryPickUp(Pet pet)
    {
        return tryPickUp(pet.getOwner());
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void unprotect()
    {
        if (_task != null)
        {
            _task.cancel(false);
        }

        _isProtected = false;
        _owner = null;
        _task = null;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void protect(Creature creature)
    {
        unprotect();

        _isProtected = true;
        _owner = creature;
        if (_owner == null)
        {
            throw new InvalidOperationException("Trying to protect dropped item to null owner");
        }

        _task = ThreadPool.schedule(this, TimeSpan.FromMilliseconds(PROTECTED_MILLIS_TIME));
    }
}