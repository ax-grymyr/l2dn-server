using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.ElementalSpirits;

public struct ExElementalSpiritEvolutionPacket: IIncomingPacket<GameSession>
{
    private ElementalType _type;

    public void ReadContent(PacketBitReader reader)
    {
        _type = (ElementalType)reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		ElementalSpirit? spirit = player.getElementalSpirit(_type);
		if (spirit == null)
		{
			connection.Send(SystemMessageId.NO_SPIRITS_ARE_AVAILABLE);
			return ValueTask.CompletedTask;
		}

		bool canEvolve = checkConditions(player, spirit);
		if (canEvolve)
		{
			spirit.upgrade();

			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_HAS_EVOLVED_TO_LV_S2);
			sm.Params.addElementalSpirit(_type).addInt(spirit.getStage());
			connection.Send(sm);

            if (!player.isSubclassLocked())
            {
                UserInfoPacket userInfo = new UserInfoPacket(player, false);
                userInfo.AddComponentType(UserInfoType.ATT_SPIRITS);
                connection.Send(userInfo);
            }
        }

		connection.Send(new ElementalSpiritEvolutionPacket(player, _type, canEvolve));

		return ValueTask.CompletedTask;
	}

    private static bool checkConditions(Player player, ElementalSpirit spirit)
    {
	    if (player.getPrivateStoreType() != PrivateStoreType.NONE)
	    {
		    player.sendPacket(SystemMessageId.CANNOT_EVOLVE_ABSORB_EXTRACT_WHILE_USING_THE_PRIVATE_STORE_WORKSHOP);
		    return false;
	    }

	    if (player.isInBattle())
	    {
		    player.sendPacket(SystemMessageId.UNABLE_TO_EVOLVE_DURING_BATTLE);
		    return false;
	    }

	    if (!spirit.canEvolve())
	    {
		    player.sendPacket(SystemMessageId.THIS_SPIRIT_CANNOT_EVOLVE);
		    return false;
	    }

	    if (!consumeEvolveItems(player, spirit))
	    {
		    player.sendPacket(SystemMessageId.NOT_ENOUGH_INGREDIENTS_FOR_EVOLUTION);
		    return false;
	    }

	    return true;
    }

    private static bool consumeEvolveItems(Player player, ElementalSpirit spirit)
	{
		PlayerInventory inventory = player.getInventory();
		try
		{
			inventory.setInventoryBlock(
				spirit.getItemsToEvolve().Select(x => x.Id).ToList(),
				InventoryBlockType.BLACKLIST);

			foreach (ItemHolder itemHolder in spirit.getItemsToEvolve())
			{
				if (inventory.getInventoryItemCount(itemHolder.Id, -1) < itemHolder.getCount())
				{
					return false;
				}
			}

			foreach (ItemHolder itemHolder in spirit.getItemsToEvolve())
			{
				player.destroyItemByItemId("Evolve", itemHolder.Id, itemHolder.getCount(), player, true);
			}

			return true;
		}
		finally
		{
			inventory.unblock();
		}
	}
}