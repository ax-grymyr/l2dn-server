using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Summon Pet effect implementation.
 * @author UnAfraid
 */
public class SummonPet: AbstractEffect
{
	public SummonPet(StatSet @params)
	{
	}

	public override EffectType getEffectType()
	{
		return EffectType.SUMMON_PET;
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
        Player? player = effector.getActingPlayer();
		if (!effector.isPlayer() || player == null || !effected.isPlayer() || effected.isAlikeDead())
		{
			return;
		}

		if (player.hasPet() || player.isMounted())
		{
			player.sendPacket(SystemMessageId.YOU_ALREADY_HAVE_A_PET);
			return;
		}

		PetItemHolder? holder = player.removeScript<PetItemHolder>();
		if (holder == null)
		{
			LOGGER.Warn( "Summoning pet without attaching PetItemHandler!");
			return;
		}

		Item collar = holder.getItem();
		if (player.getInventory().getItemByObjectId(collar.ObjectId) != collar)
		{
			LOGGER.Warn("Player: " + player + " is trying to summon pet from item that he doesn't owns.");
			return;
		}

		PetEvolveHolder evolveData = player.getPetEvolve(collar.ObjectId);
		PetData petData = evolveData.getEvolve() == EvolveLevel.None ? PetDataTable.getInstance().getPetDataByEvolve(collar.getId(), evolveData.getEvolve()) : PetDataTable.getInstance().getPetDataByEvolve(collar.getId(), evolveData.getEvolve(), evolveData.getIndex());
		if (petData == null || petData.getNpcId() == -1)
		{
			return;
		}

		NpcTemplate? npcTemplate = NpcData.getInstance().getTemplate(petData.getNpcId());
        if (npcTemplate == null)
        {
            LOGGER.Error($"NPC template id={petData.getNpcId()} not found");
            return;
        }

		Pet pet = Pet.spawnPet(npcTemplate, player, collar);
		player.setPet(pet);
		pet.setShowSummonAnimation(true);

		// Pets must have their master buffs upon spawn.
		foreach (BuffInfo effect in player.getEffectList().getEffects())
		{
			Skill sk = effect.getSkill();
			if (!sk.isBad() && !sk.isTransformation() && skill.isSharedWithSummon())
			{
				sk.applyEffects(player, pet, false, effect.getTime() ?? TimeSpan.Zero);
			}
		}

		if (!pet.isRespawned())
		{
			pet.setCurrentHp(pet.getMaxHp());
			pet.setCurrentMp(pet.getMaxMp());
			pet.getStat().setExp(pet.getExpForThisLevel());
			pet.setCurrentFed(pet.getMaxFed());
			pet.storeMe();
		}
		pet.setRunning();
		collar.setEnchantLevel(pet.getLevel());
		pet.spawnMe(new Location3D(player.getX() + 50, player.getY() + 100, player.getZ()));
		pet.startFeed();
	}
}