using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Attackables;

namespace L2Dn.GameServer.Scripts.AI.Areas;

public sealed class PlainsOfGlory: AbstractScript
{
    // Monsters
    private const int VANOR_SILENOS = 20681;
    private const int VANOR_SILENOS_SOLDIER = 20682;
    private const int VANOR_SILENOS_SCOUT = 20683;
    private const int VANOR_SILENOS_WARRIOR = 20684;
    private const int VANOR_SILENOS_SHAMAN = 20685;
    private const int VANOR_SILENOS_CHIEFTAIN = 20686;
    private const int VANOR = 24014;

    // Guard
    private const int GUARD_OF_HONOR = 22102;

    public PlainsOfGlory()
    {
        SubscribeToEvent<OnAttackableKill>(OnKill, SubscriptionType.NpcTemplate, VANOR_SILENOS,
            VANOR_SILENOS_SOLDIER, VANOR_SILENOS_SCOUT, VANOR_SILENOS_WARRIOR, VANOR_SILENOS_SHAMAN,
            VANOR_SILENOS_CHIEFTAIN, VANOR);
    }

    private void OnKill(OnAttackableKill onAttackableKill)
    {
        Npc npc = onAttackableKill.getTarget();
        Player killer = onAttackableKill.getAttacker();
        bool isSummon = onAttackableKill.isSummon();
        if (getRandom(100) < 10)
        {
            Npc spawnBanshee = addSpawn(GUARD_OF_HONOR, npc.Location, false, TimeSpan.FromMilliseconds(300000));
            Playable attacker = isSummon ? killer.getServitors().Values.FirstOrDefault() ?? killer.getPet() : killer;
            addAttackPlayerDesire(spawnBanshee, attacker);
            npc.deleteMe();
        }
    }
}