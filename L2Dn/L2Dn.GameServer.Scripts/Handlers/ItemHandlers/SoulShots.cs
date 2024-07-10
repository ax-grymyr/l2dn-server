using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

public class SoulShots: IItemHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(SoulShots));

	public bool useItem(Playable playable, Item item, bool forceUse)
	{
		if (!playable.isPlayer())
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}
		
		Player player = playable.getActingPlayer();
		Item weaponInst = player.getActiveWeaponInstance();
		Weapon weaponItem = player.getActiveWeaponItem();
		List<ItemSkillHolder> skills = item.getTemplate().getSkills(ItemSkillType.NORMAL);
		if (skills == null)
		{
			_logger.Warn(GetType().Name + ": is missing skills!");
			return false;
		}
		
		int itemId = item.getId();
		
		// Check if Soul shot can be used
		if (weaponInst == null || weaponItem.getSoulShotCount() == 0)
		{
			if (!player.getAutoSoulShot().Contains(itemId))
			{
				player.sendPacket(SystemMessageId.YOU_CANNOT_USE_SOULSHOTS);
			}
			return false;
		}
		
		// Check if Soul shot is already active
		if (player.isChargedShot(ShotType.SOULSHOTS))
		{
			return summonUseItem(playable, item);
		}
		
		// Consume Soul shots if player has enough of them
		int ssCount = weaponItem.getSoulShotCount();
		if (weaponItem.getReducedSoulShot() > 0 && Rnd.get(100) < weaponItem.getReducedSoulShotChance())
		{
			ssCount = weaponItem.getReducedSoulShot();
		}
		
		if (!player.destroyItemWithoutTrace("Consume", item.getObjectId(), ssCount, null, false))
		{
			if (!player.disableAutoShot(itemId))
			{
				player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_SOULSHOTS_FOR_THAT);
			}
			return false;
		}
		
		// Charge soul shot
		player.chargeShot(ShotType.SOULSHOTS);
		
		// Send message to client
		if (!player.getAutoSoulShot().Contains(item.getId()))
		{
			player.sendPacket(SystemMessageId.YOUR_SOULSHOTS_ARE_ENABLED);
		}
		
		// Visual effect change if player has equipped Ruby level 3 or higher
		BroochJewel? activeRubyJewel = player.getActiveRubyJewel(); 
		if (activeRubyJewel != null)
		{
			SkillHolder skill = activeRubyJewel.Value.GetSkill();
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
		if (pet != null && pet.isDead())
		{
			activeOwner.sendPacket(SystemMessageId.SOULSHOTS_AND_SPIRITSHOTS_ARE_NOT_AVAILABLE_FOR_A_DEAD_SERVITOR_SAD_ISN_T_IT);
			return false;
		}
		
		List<Summon> aliveServitor = new();
		foreach (Summon s in playable.getServitors().values())
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
		long shotCount = item.getCount();
		List<ItemSkillHolder> skills = item.getTemplate().getSkills(ItemSkillType.NORMAL);
		short shotConsumption = 0;
		if (pet != null && !pet.isChargedShot(ShotType.SOULSHOTS))
		{
			shotConsumption += pet.getSoulShotsPerHit();
		}
		
		foreach (Summon servitors in aliveServitor)
		{
			if (!servitors.isChargedShot(ShotType.SOULSHOTS))
			{
				shotConsumption += servitors.getSoulShotsPerHit();
			}
		}
		
		if (skills == null)
		{
			_logger.Warn(GetType().Name + ": is missing skills!");
			return false;
		}
		
		if (shotCount < shotConsumption)
		{
			// Not enough Soulshots to use.
			if (!activeOwner.disableAutoShot(itemId))
			{
				activeOwner.sendPacket(SystemMessageId.YOU_DON_T_HAVE_ENOUGH_SOULSHOTS_NEEDED_FOR_A_SERVITOR);
			}
			return false;
		}
		
		// If the player doesn't have enough beast soulshot remaining, remove any auto soulshot task.
		if (!activeOwner.destroyItemWithoutTrace("Consume", item.getObjectId(), shotConsumption, null, false))
		{
			if (!activeOwner.disableAutoShot(itemId))
			{
				activeOwner.sendPacket(SystemMessageId.YOU_DON_T_HAVE_ENOUGH_SOULSHOTS_NEEDED_FOR_A_SERVITOR);
			}
			return false;
		}
		
		// Pet uses the power of spirit.
		if (pet != null && !pet.isChargedShot(ShotType.SOULSHOTS))
		{
			activeOwner.sendMessage("Your pet uses soulshot."); // activeOwner.sendPacket(SystemMessageId.YOUR_PET_USES_SPIRITSHOT);
			pet.chargeShot(ShotType.SOULSHOTS);
			// Visual effect change if player has equipped Ruby level 3 or higher
			BroochJewel? activeRubyJewel = activeOwner.getActiveRubyJewel(); 
			if (activeRubyJewel != null)
			{
				SkillHolder skill = activeRubyJewel.Value.GetSkill();
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
			if (!s.isChargedShot(ShotType.SOULSHOTS))
			{
				activeOwner.sendMessage("Your servitor uses soulshot."); // activeOwner.sendPacket(SystemMessageId.YOUR_PET_USES_SPIRITSHOT);
				s.chargeShot(ShotType.SOULSHOTS);
				// Visual effect change if player has equipped Ruby level 3 or higher
				BroochJewel? activeRubyJewel = activeOwner.getActiveRubyJewel(); 
				if (activeRubyJewel != null)
				{
					SkillHolder skill = activeRubyJewel.Value.GetSkill();
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
