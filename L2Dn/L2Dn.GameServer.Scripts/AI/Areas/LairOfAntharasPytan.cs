﻿using L2Dn.GameServer.Model.Actor;
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
        Player? killer = onAttackableKill.getAttacker();
        if (killer == null)
            return;

        if (getRandom(100) < 5)
        {
            Npc npc = onAttackableKill.getTarget();
            bool isSummon = onAttackableKill.isSummon();

            Npc? spawnBanshee = addSpawn(KNORIKS, npc.Location, false, TimeSpan.FromMilliseconds(300000));
            Playable attacker = isSummon
                ? (Playable?)killer.getServitors().Values.FirstOrDefault() ?? (Playable?)killer.getPet() ?? killer
                : killer;

            if (spawnBanshee != null)
                addAttackPlayerDesire(spawnBanshee, attacker);
            
            npc.deleteMe();
        }
    }
}