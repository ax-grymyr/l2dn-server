using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerBaseStats.
 * @author mkizub
 */
public sealed class ConditionPlayerBaseStats(BaseStat stat, int value): Condition
{
    /**
     * Test impl.
     * @return true, if successful
     */
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        if (player is null)
            return false;

        return stat switch
        {
            BaseStat.INT => player.getINT() >= value,
            BaseStat.STR => player.getSTR() >= value,
            BaseStat.CON => player.getCON() >= value,
            BaseStat.DEX => player.getDEX() >= value,
            BaseStat.MEN => player.getMEN() >= value,
            BaseStat.WIT => player.getWIT() >= value,
            _ => false,
        };
    }
}