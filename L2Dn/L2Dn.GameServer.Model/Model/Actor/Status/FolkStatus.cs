using L2Dn.GameServer.Model.Actor.Instances;

namespace L2Dn.GameServer.Model.Actor.Status;

public class FolkStatus: NpcStatus
{
    public FolkStatus(Npc activeChar): base(activeChar)
    {
    }

    public override void reduceHp(double value, Creature attacker)
    {
        reduceHp(value, attacker, true, false, false);
    }

    public override void reduceHp(double value, Creature? attacker, bool awake, bool isDOT, bool isHpConsumption)
    {
    }

    public override Folk getActiveChar()
    {
        return (Folk)base.getActiveChar();
    }
}