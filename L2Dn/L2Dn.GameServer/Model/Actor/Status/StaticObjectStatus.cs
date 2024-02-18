using L2Dn.GameServer.Model.Actor.Instances;

namespace L2Dn.GameServer.Model.Actor.Status;

public class StaticObjectStatus: CreatureStatus
{
    public StaticObjectStatus(StaticObject activeChar): base(activeChar)
    {
    }

    public override StaticObject getActiveChar()
    {
        return (StaticObject)base.getActiveChar();
    }
}