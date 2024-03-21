using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExRemoveItemAttributePacket: IIncomingPacket<GameSession>
{
    private int _objectId;
    private long _price;
    private AttributeType _element;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
        _element = (AttributeType)reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

 		Item targetItem = player.getInventory().getItemByObjectId(_objectId);
		if (targetItem == null)
			return ValueTask.CompletedTask;
		
		if (!Enum.IsDefined(_element))
			return ValueTask.CompletedTask;
		
		if (targetItem.getAttributes() == null || targetItem.getAttribute(_element) == null)
			return ValueTask.CompletedTask;
		
		if (player.reduceAdena("RemoveElement", getPrice(targetItem), player, true))
		{
			targetItem.clearAttribute(_element);
			player.updateUserInfo();
			
			InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(targetItem, ItemChangeType.MODIFIED));
			player.sendInventoryUpdate(iu);

			SystemMessagePacket sm;
			AttributeType realElement = targetItem.isArmor() ? _element.getOpposite() : _element;
			if (targetItem.getEnchantLevel() > 0)
			{
				if (targetItem.isArmor())
				{
					sm = new SystemMessagePacket(SystemMessageId.S3_POWER_HAS_BEEN_REMOVED_FROM_S1_S2_S4_RESISTANCE_IS_DECREASED);
				}
				else
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_S2_S_S3_ATTRIBUTE_HAS_BEEN_REMOVED);
				}
				sm.Params.addInt(targetItem.getEnchantLevel());
				sm.Params.addItemName(targetItem);
				sm.Params.addAttribute(realElement);
				if (targetItem.isArmor())
				{
					sm.Params.addAttribute(realElement);
				}
			}
			else
			{
				if (targetItem.isArmor())
				{
					sm = new SystemMessagePacket(SystemMessageId.S2_POWER_HAS_BEEN_REMOVED_FROM_S1_S3_RESISTANCE_IS_DECREASED);
				}
				else
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_S_S2_ATTRIBUTE_HAS_BEEN_REMOVED);
				}
				
				sm.Params.addItemName(targetItem);
				if (targetItem.isArmor())
				{
					sm.Params.addAttribute(realElement);
					sm.Params.addAttribute(realElement.getOpposite());
				}
			}
			
			player.sendPacket(sm);
			player.sendPacket(new ExBaseAttributeCancelResultPacket(targetItem.getObjectId(), _element));
		}
		else
		{
			player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_FUNDS_TO_CANCEL_THIS_ATTRIBUTE);
		}

		return ValueTask.CompletedTask;
	}
	
	private static long getPrice(Item item)
	{
		long price = 0;
		switch (item.getTemplate().getCrystalType())
		{
			case CrystalType.S:
			{
				if (item.getTemplate() is Weapon)
				{
					price = 50000;
				}
				else
				{
					price = 40000;
				}
				break;
			}
			case CrystalType.S80:
			{
				if (item.getTemplate() is Weapon)
				{
					price = 100000;
				}
				else
				{
					price = 80000;
				}
				break;
			}
			case CrystalType.S84:
			{
				if (item.getTemplate() is Weapon)
				{
					price = 200000;
				}
				else
				{
					price = 160000;
				}
				break;
			}
			case CrystalType.R:
			{
				if (item.getTemplate() is Weapon)
				{
					price = 400000;
				}
				else
				{
					price = 320000;
				}
				break;
			}
			case CrystalType.R95:
			{
				if (item.getTemplate() is Weapon)
				{
					price = 800000;
				}
				else
				{
					price = 640000;
				}
				break;
			}
			case CrystalType.R99:
			{
				if (item.getTemplate() is Weapon)
				{
					price = 3200000;
				}
				else
				{
					price = 2560000;
				}
				break;
			}
		}
		
		return price;
	}
}