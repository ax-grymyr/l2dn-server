namespace L2Dn.GameServer.Model.Actor.Stats;

public class NpcStat: CreatureStat
{
    public NpcStat(Npc activeChar): base(activeChar)
    {
    }

    public override int getLevel()
    {
        return getActiveChar().getTemplate().getLevel();
    }

    public override Npc getActiveChar()
    {
        return (Npc)base.getActiveChar();
    }

    public override int getPhysicalAttackAngle()
    {
        return getActiveChar().getTemplate().getBaseAttackAngle();
    }
}