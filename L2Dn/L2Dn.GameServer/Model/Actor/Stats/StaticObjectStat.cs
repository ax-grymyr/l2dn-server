using L2Dn.GameServer.Model.Actor.Instances;

namespace L2Dn.GameServer.Model.Actor.Stats;

public class StaticObjectStat: CreatureStat
{
    public StaticObjectStat(StaticObject activeChar): base(activeChar)
    {
    }

    public override StaticObject getActiveChar()
    {
        return (StaticObject)base.getActiveChar();
    }

    public override int getLevel()
    {
        return getActiveChar().getLevel();
    }
}