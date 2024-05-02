using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Attackables;

namespace L2Dn.GameServer.Scripts.AI.Areas;

public class LairOfAntharasPytan: AbstractScript
{
    // NPCs
    private const int PYTAN = 20761;
    private const int KNORIKS = 20405;

    public LairOfAntharasPytan()
    {
        SubscribeToEvent<OnAttackableKill>(OnKill, SubscriptionType.NpcTemplate, PYTAN);
    }

    private void OnKill(OnAttackableKill onAttackableKill)
    {
        if (getRandom(100) < 5)
        {
            Npc npc = onAttackableKill.getTarget();
            Player killer = onAttackableKill.getAttacker();
            bool isSummon = onAttackableKill.isSummon();

            Npc spawnBanshee = addSpawn(KNORIKS, npc.getLocation().ToLocation3D(), npc.getLocation().Heading, false, TimeSpan.FromMilliseconds(300000));
            Playable attacker = isSummon ? killer.getServitors().Values.FirstOrDefault() ?? killer.getPet() : killer;
            addAttackPlayerDesire(spawnBanshee, attacker);
            npc.deleteMe();
        }
    }
}