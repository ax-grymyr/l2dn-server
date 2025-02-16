using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Disarm effect implementation.
 * @author nBd
 */
public class Disarm: AbstractEffect
{
	private static readonly Map<int, int> _disarmedPlayers = new();
	
	public Disarm(StatSet @params)
	{
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return effected.isPlayer();
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.DISARMED.getMask();
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		Player player = effected.getActingPlayer();
		if (player == null)
		{
			return;
		}
		
		Item itemToDisarm = player.getInventory().getPaperdollItem(Inventory.PAPERDOLL_RHAND);
		if (itemToDisarm == null)
		{
			return;
		}
		
		long slot = player.getInventory().getSlotFromItem(itemToDisarm);
		player.getInventory().unEquipItemInBodySlot(slot);
		
		InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(itemToDisarm, ItemChangeType.MODIFIED));
		player.sendInventoryUpdate(iu);
		player.broadcastUserInfo();
		
		_disarmedPlayers.put(player.ObjectId, itemToDisarm.ObjectId);
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		Player player = effected.getActingPlayer();
		if (player == null)
		{
			return;
		}
		
		int itemObjectId = _disarmedPlayers.remove(effected.ObjectId);
		if (itemObjectId == null)
		{
			return;
		}
		
		Item item = player.getInventory().getItemByObjectId(itemObjectId);
		if (item == null)
		{
			return;
		}
		
		player.getInventory().equipItem(item);
		InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(item, ItemChangeType.MODIFIED));
		player.sendInventoryUpdate(iu);
	}
}