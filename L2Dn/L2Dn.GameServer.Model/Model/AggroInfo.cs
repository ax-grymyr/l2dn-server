using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model;

public class AggroInfo
{
    private const long MAX_VALUE = 1000000000000000L;
	
    private readonly Creature _attacker;
    private long _hate = 0;
    private long _damage = 0;
	
    public AggroInfo(Creature pAttacker)
    {
        _attacker = pAttacker;
    }
	
    public Creature getAttacker()
    {
        return _attacker;
    }
	
    public long getHate()
    {
        return _hate;
    }
	
    public long checkHate(Creature owner)
    {
        if (_attacker.isAlikeDead() || !_attacker.isSpawned() || !owner.isInSurroundingRegion(_attacker))
        {
            _hate = 0;
        }
        return _hate;
    }
	
    public void addHate(long value)
    {
        _hate = Math.Min(_hate + value, MAX_VALUE);
    }
	
    public void stopHate()
    {
        _hate = 0;
    }
	
    public long getDamage()
    {
        return _damage;
    }
	
    public void addDamage(long value)
    {
        _damage = Math.Min(_damage + value, MAX_VALUE);
    }
	
    public override bool Equals(object? obj)
    {
        if (this == obj)
        {
            return true;
        }
		
        if (obj is AggroInfo)
        {
            return (((AggroInfo) obj).getAttacker() == _attacker);
        }
		
        return false;
    }
	
    public override int GetHashCode()
    {
        return _attacker.getObjectId();
    }
	
    public override string ToString()
    {
        return "AggroInfo [attacker=" + _attacker + ", hate=" + _hate + ", damage=" + _damage + "]";
    }
}
