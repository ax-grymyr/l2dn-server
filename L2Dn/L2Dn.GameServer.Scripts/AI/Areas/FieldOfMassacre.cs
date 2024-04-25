using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Attackables;

namespace L2Dn.GameServer.Scripts.AI.Areas;

public class FieldOfMassacre: AbstractScript
{
    // Monsters
    private const int DOOM_KNIGHT = 20674;
    private const int ACHER_OF_DESTRUCTION = 21001;
    private const int DOOM_SCOUT = 21002;
    private const int GRAVEYARD_LICH = 21003;
    private const int DISMAL_OAK = 21004;
    private const int GRAVEYARD_PREDATOR = 21005;
    private const int DOOM_SERVANT = 21006;
    private const int DOOM_GUARD = 21007;
    private const int DOOM_ARCHER = 21008;
    private const int DOOM_TROOPER = 21009;
    private const int DOOM_WARRIOR = 21010;

    // Guard
    private const int GUARD_BUTCHER = 22101;

    private FieldOfMassacre()
    {
        SubscribeToEvent<OnAttackableKill>(OnKill, SubscriptionType.NpcTemplate, ACHER_OF_DESTRUCTION, GRAVEYARD_LICH,
            DISMAL_OAK, GRAVEYARD_PREDATOR, DOOM_KNIGHT, DOOM_SCOUT, DOOM_SERVANT, DOOM_GUARD, DOOM_ARCHER,
            DOOM_TROOPER, DOOM_WARRIOR);
    }

    private void OnKill(OnAttackableKill onAttackableKill)
    {
        if (getRandom(100) < 10)
        {
            Npc npc = onAttackableKill.getTarget();
            Player killer = onAttackableKill.getAttacker();
            bool isSummon = onAttackableKill.isSummon();

            Npc spawnBanshee = addSpawn(GUARD_BUTCHER, npc, false, TimeSpan.FromMilliseconds(300000));
            Playable attacker = isSummon ? killer.getServitors().Values.FirstOrDefault() ?? killer.getPet() : killer;
            addAttackPlayerDesire(spawnBanshee, attacker);
            npc.deleteMe();
        }
    }
}