using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Handlers.ItemHandlers;

/**
 * Beast SpiritShot Handler
 * @author Tempy
 */
public class BeastSpiritShot: IItemHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(BeastSpiritShot));

	public bool useItem(Playable playable, Item item, bool forceUse)
	{
		if (!playable.isPlayer())
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}

		Player activeOwner = playable.getActingPlayer();
		if (!activeOwner.hasSummon())
		{
			activeOwner.sendPacket(SystemMessageId.SERVITORS_ARE_NOT_AVAILABLE_AT_THIS_TIME);
			return false;
		}

		Summon pet = playable.getPet();
		if ((pet != null) && pet.isDead())
		{
			activeOwner.sendPacket(SystemMessageId
				.SOULSHOTS_AND_SPIRITSHOTS_ARE_NOT_AVAILABLE_FOR_A_DEAD_SERVITOR_SAD_ISN_T_IT);
			return false;
		}

		List<Summon> aliveServitor = new();
		foreach (Summon s in playable.getServitors().values())
		{
			if (!s.isDead())
			{
				aliveServitor.add(s);
			}
		}

		if ((pet == null) && aliveServitor.isEmpty())
		{
			activeOwner.sendPacket(SystemMessageId
				.SOULSHOTS_AND_SPIRITSHOTS_ARE_NOT_AVAILABLE_FOR_A_DEAD_SERVITOR_SAD_ISN_T_IT);
			return false;
		}

		int itemId = item.getId();
		bool isBlessed = ((itemId == 6647) || (itemId == 20334)); // TODO: Unhardcode these!
		List<ItemSkillHolder> skills = item.getTemplate().getSkills(ItemSkillType.NORMAL);
		ShotType shotType = isBlessed ? ShotType.BLESSED_SPIRITSHOTS : ShotType.SPIRITSHOTS;
		short shotConsumption = 0;
		if ((pet != null) && !pet.isChargedShot(shotType))
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

		if (!activeOwner.destroyItemWithoutTrace("Consume", item.getObjectId(), shotConsumption, null, false))
		{
			if (!activeOwner.disableAutoShot(itemId))
			{
				activeOwner.sendPacket(SystemMessageId.YOU_DON_T_HAVE_ENOUGH_SPIRITSHOTS_FOR_THE_SERVITOR);
			}

			return false;
		}

		// Pet uses the power of spirit.
		if ((pet != null) && !pet.isChargedShot(shotType))
		{
			activeOwner.sendMessage(isBlessed
				? "Your pet uses blessed spiritshot."
				: "Your pet uses spiritshot."); // activeOwner.sendPacket(SystemMessageId.YOUR_PET_USES_SPIRITSHOT);
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
				skills.forEach(holder => Broadcast.toSelfAndKnownPlayersInRadius(activeOwner,
					new MagicSkillUsePacket(pet, pet, holder.getSkillId(), holder.getSkillLevel(), TimeSpan.Zero, TimeSpan.Zero), 600));
			}
		}

		aliveServitor.forEach(s =>
		{
			if (!s.isChargedShot(shotType))
			{
				activeOwner.sendMessage(isBlessed
					? "Your servitor uses blessed spiritshot."
					: "Your servitor uses spiritshot."); // activeOwner.sendPacket(SystemMessageId.YOUR_PET_USES_SPIRITSHOT);
				
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
					skills.forEach(holder => Broadcast.toSelfAndKnownPlayersInRadius(activeOwner,
						new MagicSkillUsePacket(s, s, holder.getSkillId(), holder.getSkillLevel(), TimeSpan.Zero, TimeSpan.Zero), 600));
				}
			}
		});
		return true;
	}
}