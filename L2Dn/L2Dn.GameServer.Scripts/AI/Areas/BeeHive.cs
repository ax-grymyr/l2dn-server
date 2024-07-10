using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Attackables;
using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Scripts.AI.Areas;

public class BeeHive: AbstractScript
{
	// NPCs
	private const int PET_70_MONSTER = 22297; // Tag [Slayer] - BUFF
	private const int PLAYER_70_MONSTER = 22303; // Rusty
	private const int PET_80_MONSTER = 22302; // Rude Tag [Slayer] - BUFF
	private const int PLAYER_80_MONSTER = 22304; // Giant Rusty

	private static readonly int[] LV_70_MONSTERS =
	[
		22293,
		22294,
		22295,
		22296,
	];

	private static readonly int[] LV_80_MONSTERS =
	[
		22298,
		22299,
		22300,
		22301,
	];

	// Skills
	private static readonly SkillHolder[] SKILLS =
	[
		new SkillHolder(48197, 1), // (Lv. 1) Pet Growth Effect
		new SkillHolder(48198, 1), // (Lv. 1) Improved Pet Skills
	];

	// Items
	private const int TAG_PET_BOX = 94634;
	private const int LOW_PET_XP_CRYSTAL = 94635;

	// Misc
	private static readonly TimeSpan DESPAWN_TIME = TimeSpan.FromMinutes(2); // 2 minutes

	private BeeHive()
	{
		SubscribeToEvent<OnAttackableKill>(OnKill, SubscriptionType.NpcTemplate, LV_70_MONSTERS);
		SubscribeToEvent<OnAttackableKill>(OnKill, SubscriptionType.NpcTemplate, LV_80_MONSTERS);
		SubscribeToEvent<OnAttackableKill>(OnKill, SubscriptionType.NpcTemplate, PET_70_MONSTER, PET_80_MONSTER);
		SubscribeToEvent<OnAttackableAttack>(OnAttack, SubscriptionType.NpcTemplate, PET_70_MONSTER, PET_80_MONSTER);
	}

	private void OnAttack(OnAttackableAttack onAttackableAttack)
	{
		if (!onAttackableAttack.isSummon())
			return;

		Pet pet = onAttackableAttack.getAttacker().getPet();
		if (pet == null || pet.getCurrentFed() == 0 || pet.isDead() || pet.isAffectedBySkill(SKILLS[0]) || pet.isAffectedBySkill(SKILLS[1]))
		{
			return;
		}

		Npc npc = onAttackableAttack.getTarget();
		if (npc.getId() == PET_70_MONSTER || npc.getId() == PET_80_MONSTER)
		{
			pet.doCast(SKILLS.GetRandomElement().getSkill());
		}
	}

	private void OnKill(OnAttackableKill onAttackableKill)
	{
		Npc npc = onAttackableKill.getTarget();
		Player killer = onAttackableKill.getAttacker();
		if (killer.hasPet() && (npc.getId() == PET_70_MONSTER || npc.getId() == PET_80_MONSTER))
		{
			if (getRandom(1000) < 1)
			{
				killer.addItem("Bee hive special monster", LOW_PET_XP_CRYSTAL, 1, killer, true);
			}
			else if (getRandom(100) < 1)
			{
				killer.addItem("Bee hive special monster", TAG_PET_BOX, 1, killer, true);
			}
		}
		else if (getRandomBoolean())
		{
			// Check if already spawned.
			foreach (Monster monster in World.getInstance().getVisibleObjects<Monster>(killer))
			{
				if (monster.getScriptValue() == killer.getObjectId())
				{
					return;
				}
			}

			bool isLow = LV_70_MONSTERS.Contains(npc.getId());
			if (isLow || LV_80_MONSTERS.Contains(npc.getId()))
			{
				Npc spawn;
				if (killer.hasPet())
				{
					spawn = addSpawn(isLow ? PET_70_MONSTER : PET_80_MONSTER, npc.Location, false, DESPAWN_TIME);
				}
				else
				{
					spawn = addSpawn(isLow ? PLAYER_70_MONSTER : PLAYER_80_MONSTER, npc.Location, false, DESPAWN_TIME);
				}
				spawn.setScriptValue(killer.getObjectId());
				spawn.setShowSummonAnimation(true);
				addAttackPlayerDesire(spawn, killer.hasPet() ? killer.getPet() : killer);
			}
		}
	}
}