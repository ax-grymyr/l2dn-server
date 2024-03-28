using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Crystalization;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Crystallization;

public struct RequestCrystallizeEstimatePacket: IIncomingPacket<GameSession>
{
    private int _objectId;
    private long _count;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
        _count = reader.ReadInt64();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		
		// if (!client.getFloodProtectors().canPerformTransaction())
		// {
		// player.sendMessage("You are crystallizing too fast.");
		// return;
		// }
		
		if (_count <= 0)
		{
			Util.handleIllegalPlayerAction(player,
				"[RequestCrystallizeItem] count <= 0! ban! oid: " + _objectId + " owner: " + player.getName(),
				Config.DEFAULT_PUNISH);
			
			return ValueTask.CompletedTask;
		}
		
		if (player.getPrivateStoreType() != PrivateStoreType.NONE || player.isInCrystallize())
		{
			player.sendPacket(SystemMessageId.WHILE_OPERATING_A_PRIVATE_STORE_OR_WORKSHOP_YOU_CANNOT_DISCARD_DESTROY_OR_TRADE_AN_ITEM);
			return ValueTask.CompletedTask;
		}
		
		int skillLevel = player.getSkillLevel((int)CommonSkill.CRYSTALLIZE);
		if (skillLevel <= 0)
		{
			player.sendPacket(SystemMessageId.YOU_MAY_NOT_CRYSTALLIZE_THIS_ITEM_YOUR_CRYSTALLIZATION_SKILL_LEVEL_IS_TOO_LOW);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		Item item = player.getInventory().getItemByObjectId(_objectId);
		if (item == null || item.isShadowItem() || item.isTimeLimitedItem() || item.isHeroItem() ||
		    (!Config.ALT_ALLOW_AUGMENT_DESTROY && item.isAugmented()))
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (!item.getTemplate().isCrystallizable() || item.getTemplate().getCrystalCount() <= 0 ||
		    item.getTemplate().getCrystalType() == CrystalType.NONE)
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			PacketLogger.Instance.Warn(player + ": tried to crystallize " + item.getTemplate());
			return ValueTask.CompletedTask;
		}

		if (_count > item.getCount())
		{
			_count = player.getInventory().getItemByObjectId(_objectId).getCount();
		}
		
		if (!player.getInventory().canManipulateWithItemId(item.getId()))
		{
			player.sendMessage("You cannot use this item.");
			return ValueTask.CompletedTask;
		}
		
		// Check if the char can crystallize items and return if false;
		bool canCrystallize = true;
		switch (item.getTemplate().getCrystalTypePlus())
		{
			case CrystalType.D:
			{
				if (skillLevel < 1)
				{
					canCrystallize = false;
				}
				break;
			}
			case CrystalType.C:
			{
				if (skillLevel < 2)
				{
					canCrystallize = false;
				}
				break;
			}
			case CrystalType.B:
			{
				if (skillLevel < 3)
				{
					canCrystallize = false;
				}
				break;
			}
			case CrystalType.A:
			{
				if (skillLevel < 4)
				{
					canCrystallize = false;
				}
				break;
			}
			case CrystalType.S:
			{
				if (skillLevel < 5)
				{
					canCrystallize = false;
				}
				break;
			}
			case CrystalType.R:
			{
				if (skillLevel < 6)
				{
					canCrystallize = false;
				}
				break;
			}
		}
		
		if (!canCrystallize)
		{
			player.sendPacket(SystemMessageId.YOU_MAY_NOT_CRYSTALLIZE_THIS_ITEM_YOUR_CRYSTALLIZATION_SKILL_LEVEL_IS_TOO_LOW);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		// Show crystallization rewards window.
		List<ItemChanceHolder> crystallizationRewards = ItemCrystallizationData.getInstance().getCrystallizationRewards(item);
		if (crystallizationRewards != null && !crystallizationRewards.isEmpty())
		{
			player.setInCrystallize(true);
			player.sendPacket(new ExGetCrystalizingEstimationPacket(crystallizationRewards));
		}
		else
		{
			player.sendPacket(SystemMessageId.ANGEL_NEVIT_S_DESCENT_BONUS_TIME_S1);
		}
        
        return ValueTask.CompletedTask;
    }
}