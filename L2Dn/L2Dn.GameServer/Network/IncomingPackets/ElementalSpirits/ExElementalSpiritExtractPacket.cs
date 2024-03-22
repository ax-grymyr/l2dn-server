using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.ElementalSpirits;

public struct ExElementalSpiritExtractPacket: IIncomingPacket<GameSession>
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

        ElementalSpirit spirit = player.getElementalSpirit(_type);
        if (spirit == null)
        {
            connection.Send(SystemMessageId.NO_SPIRITS_ARE_AVAILABLE);
            return ValueTask.CompletedTask;
        }
		
        bool canExtract = checkConditions(player, spirit);
        if (canExtract)
        {
            int amount = spirit.getExtractAmount();
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.EXTRACTED_S1_S2_SUCCESSFULLY);
            sm.Params.addItemName(spirit.getExtractItem()).addInt(amount);
            connection.Send(sm);
            
            spirit.reduceLevel();
            player.addItem("ElementalSpiritExtract", spirit.getExtractItem(), amount, player, true);
			
            UserInfoPacket userInfo = new UserInfoPacket(player);
            userInfo.addComponentType(UserInfoType.ATT_SPIRITS);
            connection.Send(userInfo);
        }
		
        connection.Send(new ElementalSpiritExtractPacket(player, _type, canExtract));

        return ValueTask.CompletedTask;
    }

    private static bool checkConditions(Player player, ElementalSpirit spirit)
    {
        if (spirit.getLevel() < 2 || spirit.getExtractAmount() < 1)
        {
            player.sendPacket(SystemMessageId.NOT_ENOUGH_ATTRIBUTE_XP_FOR_EXTRACTION);
            return false;
        }

        if (!player.getInventory().validateCapacity(1))
        {
            player.sendPacket(SystemMessageId.UNABLE_TO_EXTRACT_BECAUSE_INVENTORY_IS_FULL);
            return false;
        }

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

        if (!player.reduceAdena("ElementalSpiritExtract", ElementalSpiritData.EXTRACT_FEES[spirit.getStage() - 1],
                player, true))
        {
            player.sendPacket(SystemMessageId.NOT_ENOUGH_MATERIALS_FOR_EXTRACTION);
            return false;
        }

        return true;
    }
}