using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Attackables;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.AI.Areas;

public sealed class PlainsOfDion: AbstractScript
{
    private readonly int[] DELU_LIZARDMEN =
    [
        21104, // Delu Lizardman Supplier
        21105, // Delu Lizardman Special Agent
        21107, // Delu Lizardman Commander
    ];

    private readonly NpcStringId[] MONSTERS_MSG =
    [
        NpcStringId.S1_HOW_DARE_YOU_INTERRUPT_OUR_FIGHT_HEY_GUYS_HELP,
        NpcStringId.S1_HEY_WE_RE_HAVING_A_DUEL_HERE,
        NpcStringId.THE_DUEL_IS_OVER_ATTACK,
        NpcStringId.KILL_THE_COWARD,
        NpcStringId.HOW_DARE_YOU_INTERRUPT_A_SACRED_DUEL_YOU_MUST_BE_TAUGHT_A_LESSON,
    ];

    private readonly NpcStringId[] MONSTERS_ASSIST_MSG =
    [
        NpcStringId.DIE_YOU_COWARD,
        NpcStringId.KILL_THE_COWARD,
        NpcStringId.WHAT_ARE_YOU_LOOKING_AT,
    ];

    public PlainsOfDion()
    {
        SubscribeToEvent<OnAttackableAttack>(OnAttack, SubscriptionType.NpcTemplate, DELU_LIZARDMEN);
    }

    private void OnAttack(OnAttackableAttack onAttackableAttack)
    {
        Npc npc = onAttackableAttack.getTarget();
        Player player = onAttackableAttack.getAttacker();
        if (npc.isScriptValue(0))
        {
            int i = getRandom(5);
            if (i < 2)
            {
                npc.broadcastSay(ChatType.NPC_GENERAL, MONSTERS_MSG[i], player.getName());
            }
            else
            {
                npc.broadcastSay(ChatType.NPC_GENERAL, MONSTERS_MSG[i]);
            }

            World.getInstance().forEachVisibleObjectInRange<Monster>(npc, npc.getTemplate().getClanHelpRange(), obj =>
            {
                if (Array.IndexOf(DELU_LIZARDMEN, obj.getId()) >= 0 && !obj.isAttackingNow() && !obj.isDead() &&
                    GeoEngine.getInstance().canSeeTarget(npc, obj))
                {
                    addAttackPlayerDesire(obj, player);
                    obj.broadcastSay(ChatType.NPC_GENERAL, MONSTERS_ASSIST_MSG[getRandom(3)]);
                }
            });

            npc.setScriptValue(1);
        }
    }
}