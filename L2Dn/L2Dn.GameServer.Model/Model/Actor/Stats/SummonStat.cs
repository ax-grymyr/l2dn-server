using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Actor.Stats;

public class SummonStat: PlayableStat
{
    public SummonStat(Summon activeChar): base(activeChar)
    {
    }

    public override Summon getActiveChar()
    {
        return (Summon)base.getActiveChar();
    }

    public override double getRunSpeed()
    {
        double val = base.getRunSpeed() + Config.Character.RUN_SPD_BOOST;

        // Apply max run speed cap.
        if (val > Config.Character.MAX_RUN_SPEED_SUMMON) // In retail maximum run speed is 350 for summons and 300 for players
        {
            return Config.Character.MAX_RUN_SPEED_SUMMON;
        }

        return val;
    }

    public override double getWalkSpeed()
    {
        double val = base.getWalkSpeed() + Config.Character.RUN_SPD_BOOST;

        // Apply max run speed cap.
        if (val > Config.Character.MAX_RUN_SPEED_SUMMON) // In retail maximum run speed is 350 for summons and 300 for players
        {
            return Config.Character.MAX_RUN_SPEED_SUMMON;
        }

        return val;
    }
}