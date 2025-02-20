namespace L2Dn.GameServer.Model.Actor.Status;

public class NpcStatus: CreatureStatus
{
    public NpcStatus(Npc activeChar): base(activeChar)
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

        if (attacker != null)
        {
            Player attackerPlayer = attacker.getActingPlayer();
            if (attackerPlayer != null && attackerPlayer.isInDuel())
            {
                attackerPlayer.setDuelState(Duel.DUELSTATE_INTERRUPTED);
            }

            // Add attackers to npc's attacker list
            getActiveChar().addAttackerToAttackByList(attacker);
        }

        base.reduceHp(value, attacker, awake, isDOT, isHpConsumption);
    }

    public override Npc getActiveChar()
    {
        return (Npc)base.getActiveChar();
    }
}
