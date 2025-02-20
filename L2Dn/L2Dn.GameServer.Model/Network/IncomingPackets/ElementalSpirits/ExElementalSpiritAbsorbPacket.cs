using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.ElementalSpirits;

public struct ExElementalSpiritAbsorbPacket: IIncomingPacket<GameSession>
{
    private ElementalType _type;
    private int _itemId;
    private int _amount;

    public void ReadContent(PacketBitReader reader)
    {
        _type = (ElementalType)reader.ReadByte();
        reader.ReadInt32(); // items for now is always 1
        _itemId = reader.ReadInt32();
        _amount = reader.ReadInt32();
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

        ElementalSpiritAbsorbItemHolder absorbItem = spirit.getAbsorbItem(_itemId);
        if (absorbItem == null)
        {
            player.sendPacket(new ElementalSpiritAbsorbPacket(player, _type, false));
            return ValueTask.CompletedTask;
        }

        bool canAbsorb = checkConditions(player, spirit);
        if (canAbsorb)
        {
            connection.Send(SystemMessageId.SUCCESSFUL_ABSORPTION);
            spirit.addExperience(absorbItem.getExperience() * _amount);

            if (!player.isSubclassLocked())
            {
                UserInfoPacket userInfo = new UserInfoPacket(player, false);
                userInfo.AddComponentType(UserInfoType.ATT_SPIRITS);
                connection.Send(userInfo);
            }
        }

        connection.Send(new ElementalSpiritAbsorbPacket(player, _type, canAbsorb));

        return ValueTask.CompletedTask;
    }

    private bool checkConditions(Player player, ElementalSpirit spirit)
    {
        if (player.getPrivateStoreType() != PrivateStoreType.NONE)
        {
            player.sendPacket(SystemMessageId.CANNOT_EVOLVE_ABSORB_EXTRACT_WHILE_USING_THE_PRIVATE_STORE_WORKSHOP);
            return false;
        }

        if (player.isInBattle())
        {
            player.sendPacket(SystemMessageId.UNABLE_TO_ABSORB_DURING_BATTLE);
            return false;
        }

        if (spirit.getLevel() == spirit.getMaxLevel() && spirit.getExperience() == spirit.getExperienceToNextLevel())
        {
            player.sendPacket(SystemMessageId.YOU_HAVE_REACHED_THE_MAXIMUM_LEVEL_AND_CANNOT_ABSORB_ANY_FURTHER);
            return false;
        }

        if (_amount < 1 || !player.destroyItemByItemId("Absorb", _itemId, _amount, player, true))
        {
            player.sendPacket(SystemMessageId.NOT_ENOUGH_INGREDIENTS_TO_ABSORB);
            return false;
        }

        return true;
    }
}