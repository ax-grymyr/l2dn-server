using L2Dn.GameServer.Model.Actor.Instances;

namespace L2Dn.GameServer.Model.Actor.Status;

public class SiegeFlagStatus: NpcStatus
{
    public SiegeFlagStatus(SiegeFlag activeChar): base(activeChar)
    {
    }

    public override void reduceHp(double value, Creature attacker)
    {
        reduceHp(value, attacker, true, false, false);
    }

    public override void reduceHp(double value, Creature attacker, bool awake, bool isDOT, bool isHpConsumption)
    {
        if (getActiveChar().isAdvancedHeadquarter())
        {
            base.reduceHp(value / 2, attacker, awake, isDOT, isHpConsumption);
        }

        base.reduceHp(value, attacker, awake, isDOT, isHpConsumption);
    }

    public override SiegeFlag getActiveChar()
    {
        return (SiegeFlag)base.getActiveChar();
    }
}