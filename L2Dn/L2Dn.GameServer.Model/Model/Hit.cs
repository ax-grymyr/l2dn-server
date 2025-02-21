using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model;

public class Hit
{
    private readonly WeakReference<WorldObject> _target;
    private readonly int _targetId;
    private readonly int _damage;
    private readonly ItemGrade _ssGrade;
    private AttackType _flags = 0;

    public Hit(WorldObject target, int damage, bool miss, bool crit, byte shld, bool soulshot, ItemGrade ssGrade)
    {
        _target = new WeakReference<WorldObject>(target);
        _targetId = target.ObjectId;
        _damage = damage;
        _ssGrade = ssGrade;

        if (miss)
        {
            addMask(AttackType.MISSED);
            return;
        }

        if (crit)
        {
            addMask(AttackType.CRITICAL);
        }

        if (soulshot)
        {
            addMask(AttackType.SHOT_USED);
        }

        if ((target.isCreature() && ((Creature) target).isHpBlocked()) || shld > 0)
        {
            addMask(AttackType.BLOCKED);
        }
    }

    private void addMask(AttackType type)
    {
        _flags |= type;
    }

    public WorldObject? getTarget()
    {
        _target.TryGetTarget(out WorldObject? target);
        return target;
    }

    public int getTargetId()
    {
        return _targetId;
    }

    public int getDamage()
    {
        return _damage;
    }

    public AttackType getFlags()
    {
        return _flags;
    }

    public ItemGrade getGrade()
    {
        return _ssGrade;
    }

    public bool isMiss()
    {
        return (AttackType.MISSED & _flags) != 0;
    }

    public bool isCritical()
    {
        return (AttackType.CRITICAL & _flags) != 0;
    }

    public bool isShotUsed()
    {
        return (AttackType.SHOT_USED & _flags) != 0;
    }

    public bool isBlocked()
    {
        return (AttackType.BLOCKED & _flags) != 0;
    }
}