using L2Dn.GameServer.AI;

namespace L2Dn.GameServer.Model.Actor.Status;

public class PetStatus: SummonStatus
{
    private int _currentFed = 0; // Current Fed of the Pet

    public PetStatus(Pet activeChar): base(activeChar)
    {
    }

    public override void reduceHp(double value, Creature attacker)
    {
        reduceHp(value, attacker, true, false, false);
    }

    public override void reduceHp(double value, Creature attacker, bool awake, bool isDOT, bool isHpConsumption)
    {
        if (getActiveChar().isDead())
        {
            return;
        }

        base.reduceHp(value, attacker, awake, isDOT, isHpConsumption);

        if (attacker != null)
        {
            if (!isDOT && (getActiveChar().getOwner() != null))
            {
                SystemMessage sm = new SystemMessage(SystemMessageId.C1_DEALS_S2_DAMAGE_TO_THE_PET);
                sm.addString(attacker.getName());
                sm.addInt((int)value);
                getActiveChar().sendPacket(sm);
            }

            getActiveChar().getAI().notifyEvent(CtrlEvent.EVT_ATTACKED, attacker);
        }
    }

    public int getCurrentFed()
    {
        return _currentFed;
    }

    public void setCurrentFed(int value)
    {
        _currentFed = value;
    }

    public override Pet getActiveChar()
    {
        return (Pet)super.getActiveChar();
    }
}
