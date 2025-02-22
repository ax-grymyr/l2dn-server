using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

public class SpiritShot: IItemHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(SpiritShot));

	public bool useItem(Playable playable, Item item, bool forceUse)
	{
        Player? player = playable.getActingPlayer();
		if (!playable.isPlayer() || player == null)
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}

		Item? weaponInst = player.getActiveWeaponInstance();
		Weapon? weaponItem = player.getActiveWeaponItem();
		List<ItemSkillHolder>? skills = item.getTemplate().getSkills(ItemSkillType.NORMAL);
		if (skills == null)
		{
			_logger.Warn(GetType().Name + ": is missing skills!");
			return false;
		}

		int itemId = item.getId();

		// Check if SpiritShot can be used
		if (weaponInst == null || weaponItem == null || weaponItem.getSpiritShotCount() == 0)
		{
			if (!player.getAutoSoulShot().Contains(itemId))
			{
				player.sendPacket(SystemMessageId.YOU_CANNOT_USE_SPIRITSHOTS);
			}
			return false;
		}

		// Check if SpiritShot is already active
		if (player.isChargedShot(ShotType.SPIRITSHOTS))
		{
			return summonUseItem(playable, item);
		}

		// Consume SpiritShot if player has enough of them
		if (!player.destroyItemWithoutTrace("Consume", item.ObjectId, weaponItem.getSpiritShotCount(), null, false))
		{
			if (!player.disableAutoShot(itemId))
			{
				player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_SPIRITSHOT_FOR_THAT);
			}
			return false;
		}

		// Charge Spirit shot
		player.chargeShot(ShotType.SPIRITSHOTS);

		// Send message to client
		if (!player.getAutoSoulShot().Contains(item.getId()))
		{
			player.sendPacket(SystemMessageId.YOUR_SPIRITSHOT_HAS_BEEN_ENABLED);
		}

		// Visual effect change if player has equipped Sapphire level 3 or higher
		BroochJewel? activeShappireJewel = player.getActiveShappireJewel();
		if (activeShappireJewel != null)
		{
			SkillHolder skill = activeShappireJewel.Value.GetSkill();
			Broadcast.toSelfAndKnownPlayersInRadius(player,
				new MagicSkillUsePacket(player, player, skill.getSkillId(),
					skill.getSkillLevel(), TimeSpan.Zero, TimeSpan.Zero), 600);
		}
		else
		{
			skills.ForEach(holder => Broadcast.toSelfAndKnownPlayersInRadius(player,
				new MagicSkillUsePacket(player, player, holder.getSkillId(), holder.getSkillLevel(), TimeSpan.Zero, TimeSpan.Zero), 600));
		}

		return true;
	}

	private bool summonUseItem(Playable playable, Item item)
	{
        Player? activeOwner = playable.getActingPlayer();
		if (!playable.isPlayer() || activeOwner == null)
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}

		if (!activeOwner.hasSummon())
		{
			activeOwner.sendPacket(SystemMessageId.SERVITORS_ARE_NOT_AVAILABLE_AT_THIS_TIME);
			return false;
		}

		Summon? pet = playable.getPet();
		if (pet != null && pet.isDead())
		{
			activeOwner.sendPacket(SystemMessageId.SOULSHOTS_AND_SPIRITSHOTS_ARE_NOT_AVAILABLE_FOR_A_DEAD_SERVITOR_SAD_ISN_T_IT);
			return false;
		}

		List<Summon> aliveServitor = new();
		foreach (Summon s in playable.getServitors().Values)
		{
			if (!s.isDead())
			{
				aliveServitor.Add(s);
			}
		}

		if (pet == null && aliveServitor.Count == 0)
		{
			activeOwner.sendPacket(SystemMessageId.SOULSHOTS_AND_SPIRITSHOTS_ARE_NOT_AVAILABLE_FOR_A_DEAD_SERVITOR_SAD_ISN_T_IT);
			return false;
		}

		int itemId = item.getId();
		bool isBlessed = itemId == 6647 || itemId == 20334; // TODO: Unhardcode these!
		ShotType shotType = isBlessed ? ShotType.BLESSED_SPIRITSHOTS : ShotType.SPIRITSHOTS;
		short shotConsumption = 0;
		if (pet != null && !pet.isChargedShot(shotType))
		{
			shotConsumption += pet.getSpiritShotsPerHit();
		}

		foreach (Summon servitors in aliveServitor)
		{
			if (!servitors.isChargedShot(shotType))
			{
				shotConsumption += servitors.getSpiritShotsPerHit();
			}
		}

        List<ItemSkillHolder>? skills = item.getTemplate().getSkills(ItemSkillType.NORMAL);
		if (skills == null)
		{
			_logger.Warn(GetType().Name + ": is missing skills!");
			return false;
		}

		long shotCount = item.getCount();
		if (shotCount < shotConsumption)
		{
			// Not enough SpiritShots to use.
			if (!activeOwner.disableAutoShot(itemId))
			{
				activeOwner.sendPacket(SystemMessageId.YOU_DON_T_HAVE_ENOUGH_SPIRITSHOTS_FOR_THE_SERVITOR);
			}
			return false;
		}

		if (!activeOwner.destroyItemWithoutTrace("Consume", item.ObjectId, shotConsumption, null, false))
		{
			if (!activeOwner.disableAutoShot(itemId))
			{
				activeOwner.sendPacket(SystemMessageId.YOU_DON_T_HAVE_ENOUGH_SPIRITSHOTS_FOR_THE_SERVITOR);
			}
			return false;
		}

		// Pet uses the power of spirit.
		if (pet != null && !pet.isChargedShot(shotType))
		{
			activeOwner.sendMessage(isBlessed ? "Your pet uses blessed spiritshot." : "Your pet uses spiritshot."); // activeOwner.sendPacket(SystemMessageId.YOUR_PET_USES_SPIRITSHOT);
			pet.chargeShot(shotType);
			// Visual effect change if player has equipped Sapphire level 3 or higher
			BroochJewel? activeShappireJewel = activeOwner.getActiveShappireJewel();
			if (activeShappireJewel != null)
			{
				SkillHolder skill = activeShappireJewel.Value.GetSkill();
				Broadcast.toSelfAndKnownPlayersInRadius(activeOwner,
					new MagicSkillUsePacket(pet, pet, skill.getSkillId(),
						skill.getSkillLevel(), TimeSpan.Zero, TimeSpan.Zero), 600);
			}
			else
			{
				skills.ForEach(holder => Broadcast.toSelfAndKnownPlayersInRadius(activeOwner,
					new MagicSkillUsePacket(pet, pet, holder.getSkillId(), holder.getSkillLevel(), TimeSpan.Zero, TimeSpan.Zero), 600));
			}
		}

		aliveServitor.ForEach(s =>
		{
			if (!s.isChargedShot(shotType))
			{
				activeOwner.sendMessage(isBlessed ? "Your servitor uses blessed spiritshot." : "Your servitor uses spiritshot."); // activeOwner.sendPacket(SystemMessageId.YOUR_PET_USES_SPIRITSHOT);
				s.chargeShot(shotType);
				// Visual effect change if player has equipped Sapphire level 3 or higher
				BroochJewel? activeShappireJewel = activeOwner.getActiveShappireJewel();
				if (activeShappireJewel != null)
				{
					SkillHolder skill = activeShappireJewel.Value.GetSkill();
					Broadcast.toSelfAndKnownPlayersInRadius(activeOwner,
						new MagicSkillUsePacket(s, s, skill.getSkillId(),
							skill.getSkillLevel(), TimeSpan.Zero, TimeSpan.Zero), 600);
				}
				else
				{
					skills.ForEach(holder => Broadcast.toSelfAndKnownPlayersInRadius(activeOwner,
						new MagicSkillUsePacket(s, s, holder.getSkillId(), holder.getSkillLevel(), TimeSpan.Zero, TimeSpan.Zero), 600));
				}
			}
		});
		return true;
	}
}