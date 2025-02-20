using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model;

public class DamageDoneInfo
{
    private readonly Player _attacker;
    private long _damage = 0;
	
    public DamageDoneInfo(Player attacker)
    {
        _attacker = attacker;
    }
	
    public Player getAttacker()
    {
        return _attacker;
    }
	
    public void addDamage(long damage)
    {
        _damage += damage;
    }
	
    public long getDamage()
    {
        return _damage;
    }
	
    public override bool Equals(object? obj)
    {
        return this == obj || (obj is DamageDoneInfo && ((DamageDoneInfo) obj).getAttacker() == _attacker);
    }
	
    public override int GetHashCode()
    {
        return _attacker.ObjectId;
    }
}
