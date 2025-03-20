using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestDropItemPacket: IIncomingPacket<GameSession>
{
    private int _objectId;
    private long _count;
    private Location3D _location;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
        _count = reader.ReadInt64();
        _location = reader.ReadLocation3D();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null || player.isDead())
		    return ValueTask.CompletedTask;

	    // TODO: Flood protect drop to avoid packet lag
	    // if (!client.getFloodProtectors().canDropItem())
	    // {
	    // 	return;
	    // }

	    Item? item = player.getInventory().getItemByObjectId(_objectId);
	    if (item == null || _count == 0 || !player.validateItemManipulation(_objectId, "drop") ||
	        (!Config.General.ALLOW_DISCARDITEM && !player.canOverrideCond(PlayerCondOverride.DROP_ALL_ITEMS)) ||
	        (!item.isDropable() && !(player.canOverrideCond(PlayerCondOverride.DROP_ALL_ITEMS) &&
	                                 Config.General.GM_TRADE_RESTRICTED_ITEMS)) ||
	        (item.getItemType() == EtcItemType.PET_COLLAR && player.havePetInvItems()) ||
	        player.isInsideZone(ZoneId.NO_ITEM_DROP))
	    {
		    connection.Send(SystemMessageId.THAT_ITEM_CANNOT_BE_DISCARDED);
		    return ValueTask.CompletedTask;
	    }

	    if (item.isQuestItem() && !(player.canOverrideCond(PlayerCondOverride.DROP_ALL_ITEMS) &&
	                                Config.General.GM_TRADE_RESTRICTED_ITEMS))
		    return ValueTask.CompletedTask;

	    if (_count > item.getCount())
	    {
		    connection.Send(SystemMessageId.THAT_ITEM_CANNOT_BE_DISCARDED);
		    return ValueTask.CompletedTask;
	    }

	    if (Config.Character.PLAYER_SPAWN_PROTECTION > 0 && player.isInvul() && !player.isGM())
	    {
		    connection.Send(SystemMessageId.THAT_ITEM_CANNOT_BE_DISCARDED);
		    return ValueTask.CompletedTask;
	    }

	    if (_count < 0)
	    {
		    Util.handleIllegalPlayerAction(player,
			    "[RequestDropItem] Character " + player.getName() + " of account " + player.getAccountName() +
			    " tried to drop item with oid " + _objectId + " but has count < 0!", Config.General.DEFAULT_PUNISH);
		    return ValueTask.CompletedTask;
	    }

	    if (!item.isStackable() && _count > 1)
	    {
		    Util.handleIllegalPlayerAction(player,
			    "[RequestDropItem] Character " + player.getName() + " of account " + player.getAccountName() +
			    " tried to drop non-stackable item with oid " + _objectId + " but has count > 1!",
			    Config.General.DEFAULT_PUNISH);
		    return ValueTask.CompletedTask;
	    }

	    if (Config.General.JAIL_DISABLE_TRANSACTION && player.isJailed())
	    {
		    player.sendMessage("You cannot drop items in Jail.");
		    return ValueTask.CompletedTask;
	    }

	    if (!player.getAccessLevel().AllowTransaction)
	    {
		    player.sendMessage("Transactions are disabled for your Access Level.");
		    connection.Send(SystemMessageId.NOTHING_HAPPENED);
		    return ValueTask.CompletedTask;
	    }

	    if (player.isProcessingTransaction() || player.getPrivateStoreType() != PrivateStoreType.NONE)
	    {
		    connection.Send(SystemMessageId
			    .WHILE_OPERATING_A_PRIVATE_STORE_OR_WORKSHOP_YOU_CANNOT_DISCARD_DESTROY_OR_TRADE_AN_ITEM);
		    return ValueTask.CompletedTask;
	    }

	    if (player.isFishing())
	    {
		    // You can't mount, dismount, break and drop items while fishing
		    connection.Send(SystemMessageId.YOU_CANNOT_DO_THAT_WHILE_FISHING_2);
		    return ValueTask.CompletedTask;
	    }

	    if (player.isFlying())
		    return ValueTask.CompletedTask;

	    if (player.hasItemRequest())
	    {
		    connection.Send(SystemMessageId.YOU_CANNOT_DESTROY_OR_CRYSTALLIZE_ITEMS_WHILE_ENCHANTING_ATTRIBUTES);
		    return ValueTask.CompletedTask;
	    }

	    // Cannot discard item that the skill is consuming.
	    if (player.isCastingNow(s =>
		        s.getSkill().ItemConsumeId == item.Id && item.getTemplate().getDefaultAction() ==
		        ActionType.SKILL_REDUCE_ON_SKILL_SUCCESS))
	    {
		    connection.Send(SystemMessageId.THAT_ITEM_CANNOT_BE_DISCARDED);
		    return ValueTask.CompletedTask;
	    }

	    if (ItemTemplate.TYPE2_QUEST == item.getTemplate().getType2() &&
	        !player.canOverrideCond(PlayerCondOverride.DROP_ALL_ITEMS))
	    {
		    connection.Send(SystemMessageId.THAT_ITEM_CANNOT_BE_DISCARDED_OR_EXCHANGED);
		    return ValueTask.CompletedTask;
	    }

	    if (!player.IsInsideRadius2D(_location.Location2D, 150) || Math.Abs(_location.Z - player.getZ()) > 50)
	    {
		    connection.Send(SystemMessageId.YOU_CANNOT_DISCARD_SOMETHING_THAT_FAR_AWAY_FROM_YOU);
		    return ValueTask.CompletedTask;
	    }

	    if (!player.getInventory().canManipulateWithItemId(item.Id))
	    {
		    player.sendMessage("You cannot use this item.");
		    return ValueTask.CompletedTask;
	    }

	    // Do not drop items when casting known skills to avoid exploits.
	    if (player.isCastingNow())
	    {
		    foreach (SkillCaster skillCaster in player.getSkillCasters())
		    {
			    Skill skill = skillCaster.getSkill();
			    if (skill != null && player.getKnownSkill(skill.Id) != null)
			    {
				    player.sendMessage("You cannot drop an item while casting " + skill.Name + ".");
				    return ValueTask.CompletedTask;
			    }
		    }
	    }

	    SkillUseHolder? skillUseHolder = player.getQueuedSkill();
	    if (skillUseHolder != null && player.getKnownSkill(skillUseHolder.getSkillId()) != null)
	    {
		    player.sendMessage("You cannot drop an item while casting " + skillUseHolder.getSkill().Name + ".");
		    return ValueTask.CompletedTask;
	    }

	    if (item.isEquipped())
	    {
		    player.getInventory().unEquipItemInSlot(item.getLocationSlot());
		    player.broadcastUserInfo();
		    player.sendItemList();
	    }

	    Item? dropedItem = player.dropItem("Drop", _objectId, _count, _location, null, false, false);

	    // player.broadcastUserInfo();
	    if (player.isGM())
	    {
		    //String target = (player.getTarget() != null ? player.getTarget().getName() : "no-target");
		    //GMAudit.auditGMAction(player.getName() + " [" + player.getObjectId() + "]", "Drop", target, "(id: " + dropedItem.getId() + " name: " + dropedItem.getItemName() + " objId: " + dropedItem.getObjectId() + " x: " + player.getX() + " y: " + player.getY() + " z: " + player.getZ() + ")");
	    }

	    if (dropedItem != null && dropedItem.Id == Inventory.ADENA_ID && dropedItem.getCount() >= 1000000)
	    {
		    string msg = $"Character ({player.getName()}) has dropped ({dropedItem.getCount()})adena at {_location}";
		    PacketLogger.Instance.Warn(msg);
		    GmManager.getInstance().BroadcastMessageToGMs(msg);
	    }

	    return ValueTask.CompletedTask;
    }
}