using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Attackables;

namespace L2Dn.GameServer.Scripts.AI.Areas;

public class Cemetery: AbstractScript
{
    // Monsters
    private const int SOUL_OF_RUINS = 21000;
    private const int ROVING_SOUL = 20999;
    private const int CRUEL_PUNISHER = 20998;
    private const int SOLDIER_OF_GRIEF = 20997;
    private const int SPITEFUL_GHOST_OF_RUINS = 20996;
    private const int TORTURED_UNDEAD = 20678;
    private const int TAIRIM = 20675;
    private const int TAIK_ORC_SUPPLY_OFFICER = 20669;
    private const int GRAVE_GUARD = 20668;
    private const int TAIK_ORC_WATCHMAN = 20666;

    // Guard
    private const int GRAVE_WARDEN = 22128;

    public Cemetery()
    {
        SubscribeToEvent<OnAttackableKill>(OnKill, SubscriptionType.NpcTemplate, SOUL_OF_RUINS, ROVING_SOUL,
            CRUEL_PUNISHER, SOLDIER_OF_GRIEF, SPITEFUL_GHOST_OF_RUINS, TORTURED_UNDEAD, TAIRIM, TAIK_ORC_SUPPLY_OFFICER,
            GRAVE_GUARD, TAIK_ORC_WATCHMAN);
    }

    private void OnKill(OnAttackableKill onAttackableKill)
    {
        Npc npc = onAttackableKill.getTarget();
        Player killer = onAttackableKill.getAttacker();
        bool isSummon = onAttackableKill.isSummon();
        if (getRandom(100) < 10)
        {
            Npc spawnBanshee = addSpawn(GRAVE_WARDEN, npc.getLocation().ToLocation3D(), npc.getLocation().Heading, false, TimeSpan.FromMilliseconds(300000));
            Playable attacker = isSummon
                ? killer.getServitors().Values.FirstOrDefault() ?? killer.getPet()
                : killer;

            addAttackPlayerDesire(spawnBanshee, attacker);
            npc.deleteMe();
        }
    }
}