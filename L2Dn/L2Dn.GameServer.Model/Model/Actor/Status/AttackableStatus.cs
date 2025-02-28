namespace L2Dn.GameServer.Model.Actor.Status;

public class AttackableStatus: NpcStatus
{
    public AttackableStatus(Attackable activeChar): base(activeChar)
    {
    }

    public override void reduceHp(double value, Creature attacker)
    {
        reduceHp(value, attacker, true, false, false);
    }

    public override void reduceHp(double value, Creature? attacker, bool awake, bool isDOT, bool isHpConsumption)
    {
        if (getActiveChar().isDead())
        {
            return;
        }

        if (value > 0)
        {
            if (getActiveChar().isOverhit())
            {
                getActiveChar().setOverhitValues(attacker, value);
            }
            else
            {
                getActiveChar().overhitEnabled(false);
            }
        }
        else
        {
            getActiveChar().overhitEnabled(false);
        }

        base.reduceHp(value, attacker, awake, isDOT, isHpConsumption);

        if (!getActiveChar().isDead())
        {
            // And the attacker's hit didn't kill the mob, clear the over-hit flag
            getActiveChar().overhitEnabled(false);
        }
    }

    public override bool setCurrentHp(double newHp, bool broadcastPacket)
    {
        return base.setCurrentHp(newHp, true);
    }

    public override Attackable getActiveChar()
    {
        return (Attackable)base.getActiveChar();
    }
}