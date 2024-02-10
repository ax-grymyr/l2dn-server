namespace L2Dn.GameServer.Model.Actor.Status;

public class DoorStatus: CreatureStatus
{
    public DoorStatus(Door activeChar): base(activeChar)
    {
    }

    public override Door getActiveChar()
    {
        return (Door)base.getActiveChar();
    }
}
