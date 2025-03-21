using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;
using L2Dn.GameServer.StaticData;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.ElementalSpirits;

public struct ExElementalInitTalentPacket: IIncomingPacket<GameSession>
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

        if (player.isInBattle())
        {
            connection.Send(SystemMessageId.UNABLE_TO_RESET_THE_SPIRIT_ATTRIBUTES_WHILE_IN_BATTLE);
            connection.Send(new ElementalSpiritSetTalentPacket(player, _type, false));
            return ValueTask.CompletedTask;
        }

        if (player.reduceAdena("Talent", ElementalSpiritData.TalentInitFee, player, true))
        {
            spirit.resetCharacteristics();
            connection.Send(SystemMessageId.RESET_THE_SELECTED_SPIRIT_S_CHARACTERISTICS_SUCCESSFULLY);
            connection.Send(new ElementalSpiritSetTalentPacket(player, _type, true));
        }
        else
        {
            connection.Send(new ElementalSpiritSetTalentPacket(player, _type, false));
        }

        return ValueTask.CompletedTask;
    }
}