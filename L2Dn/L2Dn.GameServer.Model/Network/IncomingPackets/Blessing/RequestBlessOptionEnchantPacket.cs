using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Blessing;
using L2Dn.GameServer.Network.OutgoingPackets.Enchanting;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Network.IncomingPackets.Blessing;

public struct RequestBlessOptionEnchantPacket: IIncomingPacket<GameSession>
{
    private int _itemObjId;

    public void ReadContent(PacketBitReader reader)
    {
        _itemObjId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		Item targetInstance = player.getInventory().getItemByObjectId(_itemObjId);
		if (targetInstance == null)
		{
			player.sendPacket(new ExBlessOptionEnchantPacket(EnchantResultPacket.ERROR));
			return ValueTask.CompletedTask;
		}
		
		BlessingItemRequest request = player.getRequest<BlessingItemRequest>();
		if (request == null || request.isProcessing())
		{
			player.sendPacket(new ExBlessOptionEnchantPacket(EnchantResultPacket.ERROR));
			return ValueTask.CompletedTask;
		}

		request.setProcessing(true);
		request.setTimestamp(DateTime.UtcNow);
		
		if (!player.isOnline() || session.IsDetached)
		{
			return ValueTask.CompletedTask;
		}
		
		if (player.isInStoreMode())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_ENCHANT_WHILE_OPERATING_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP);
			player.sendPacket(new ExBlessOptionEnchantPacket(EnchantResultPacket.ERROR));
			return ValueTask.CompletedTask;
		}
		
		Item item = player.getInventory().getItemByObjectId(_itemObjId);
		if (item == null)
		{
			player.sendPacket(new ExBlessOptionEnchantPacket(EnchantResultPacket.ERROR));
			return ValueTask.CompletedTask;
		}
		
		// first validation check - also over enchant check
		if (item.isBlessed())
		{
			player.sendPacket(SystemMessageId.AUGMENTATION_REQUIREMENTS_ARE_NOT_FULFILLED);
			player.sendPacket(new ExBlessOptionPutItemPacket(0));
			return ValueTask.CompletedTask;
		}
		
		Item targetScroll = player.getInventory().getItemByItemId(request.getBlessScrollId());
		if (targetScroll == null)
		{
			player.sendPacket(new ExBlessOptionEnchantPacket(EnchantResultPacket.ERROR));
			return ValueTask.CompletedTask;
		}
		
		// attempting to destroy scroll
		if (player.getInventory().destroyItem("Blessing", targetScroll.getObjectId(), 1, player, item) == null)
		{
			player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
			Util.handleIllegalPlayerAction(player, player + " tried to bless with a scroll he doesn't have", Config.DEFAULT_PUNISH);
			player.sendPacket(new ExBlessOptionEnchantPacket(EnchantResultPacket.ERROR));
			return ValueTask.CompletedTask;
		}
		
		if (Rnd.get(100) < Config.BLESSING_CHANCE) // Success
		{
			ItemTemplate it = item.getTemplate();
			// Increase enchant level only if scroll's base template has chance, some armors can success over +20 but they shouldn't have increased.
			item.setBlessed(true);
			item.updateDatabase();
			player.sendPacket(new ExBlessOptionEnchantPacket(1));
			// Announce the success.
			if (item.getEnchantLevel() >= (item.isArmor() ? Config.MIN_ARMOR_ENCHANT_ANNOUNCE : Config.MIN_WEAPON_ENCHANT_ANNOUNCE) //
				&& item.getEnchantLevel() <= (item.isArmor() ? Config.MAX_ARMOR_ENCHANT_ANNOUNCE : Config.MAX_WEAPON_ENCHANT_ANNOUNCE))
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_ENCHANTED_S3_UP_TO_S2);
				sm.Params.addString(player.getName());
				sm.Params.addInt(item.getEnchantLevel());
				sm.Params.addItemName(item);
				player.broadcastPacket(sm);
				Broadcast.toAllOnlinePlayers(new ExItemAnnouncePacket(player, item, ExItemAnnouncePacket.ENCHANT));
				
				Skill skill = CommonSkill.FIREWORK.getSkill();
				if (skill != null)
				{
					player.broadcastPacket(new MagicSkillUsePacket(player, player, skill.getId(), skill.getLevel(),
						skill.getHitTime(), skill.getReuseDelay()));
				}
			}
			if (item.isEquipped())
			{
				if (item.isArmor())
				{
					it.forEachSkill(ItemSkillType.ON_BLESSING, holder =>
					{
						player.addSkill(holder.getSkill(), false);
						player.sendSkillList();
					});
				}
				player.broadcastUserInfo();
			}
		}
		else // Failure.
		{
			player.sendPacket(new ExBlessOptionEnchantPacket(0));
		}
		
		request.setProcessing(false);
		player.sendItemList();
		player.broadcastUserInfo();
        
        return ValueTask.CompletedTask;
    }
}