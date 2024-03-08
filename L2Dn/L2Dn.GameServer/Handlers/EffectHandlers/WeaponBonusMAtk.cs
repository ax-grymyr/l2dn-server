using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sero
 */
public class WeaponBonusMAtk: AbstractStatAddEffect
{
	public WeaponBonusMAtk(StatSet @params): base(@params, Stat.WEAPON_BONUS_MAGIC_ATTACK)
	{
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		Player player = effected.getActingPlayer();
		if (player == null)
		{
			return;
		}
		
		Item weapon = player.getInventory().getPaperdollItem(Inventory.PAPERDOLL_RHAND);
		if (weapon == null)
		{
			return;
		}
		
		InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(weapon, ItemChangeType.MODIFIED));
		player.sendInventoryUpdate(iu);
		player.broadcastUserInfo();
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		Player player = effected.getActingPlayer();
		if (player == null)
		{
			return;
		}
		
		Item weapon = player.getInventory().getPaperdollItem(Inventory.PAPERDOLL_RHAND);
		if (weapon == null)
		{
			return;
		}
		
		InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(weapon, ItemChangeType.MODIFIED));
		player.sendInventoryUpdate(iu);
		player.broadcastUserInfo();
	}
}