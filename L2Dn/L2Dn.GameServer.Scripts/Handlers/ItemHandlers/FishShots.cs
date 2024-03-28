using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * @author -Nemesiss-
 */
public class FishShots: IItemHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(FishShots));
	
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
		if ((weaponInst == null) || (weaponItem.getItemType() != WeaponType.FISHINGROD))
		{
			return false;
		}
		
		if (player.isChargedShot(ShotType.FISH_SOULSHOTS))
		{
			return false;
		}
		
		long count = item.getCount();
		bool gradeCheck = item.isEtcItem() && (item.getEtcItem().getDefaultAction() == ActionType.FISHINGSHOT) && (weaponInst.getTemplate().getCrystalTypePlus() == item.getTemplate().getCrystalTypePlus());
		if (!gradeCheck)
		{
			player.sendPacket(SystemMessageId.THAT_IS_THE_WRONG_GRADE_OF_SOULSHOT_FOR_THAT_FISHING_POLE);
			return false;
		}
		
		if (count < 1)
		{
			return false;
		}
		
		player.chargeShot(ShotType.FISH_SOULSHOTS);
		player.destroyItemWithoutTrace("Consume", item.getObjectId(), 1, null, false);
		WorldObject oldTarget = player.getTarget();
		player.setTarget(player);
		
		List<ItemSkillHolder> skills = item.getTemplate().getSkills(ItemSkillType.NORMAL);
		if (skills == null)
		{
			_logger.Warn(GetType().Name + ": is missing skills!");
			return false;
		}

		skills.forEach(holder => Broadcast.toSelfAndKnownPlayersInRadius(player,
			new MagicSkillUsePacket(player, player, holder.getSkillId(), holder.getSkillLevel(), TimeSpan.Zero, TimeSpan.Zero), 600));
		player.setTarget(oldTarget);
		
		return true;
	}
}