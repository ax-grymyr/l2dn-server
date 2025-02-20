using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.ElementalSpirits;

public struct ExElementalSpiritSetTalentPacket: IIncomingPacket<GameSession>
{
    private ElementalType _type;
    private byte _attackPoints;
    private byte _defensePoints;
    private byte _critRate;
    private byte _critDamage;

    public void ReadContent(PacketBitReader reader)
    {
        _type = (ElementalType)reader.ReadByte();
        reader.ReadByte(); // Characteristics for now always 4

        reader.ReadByte(); // attack id
        _attackPoints = reader.ReadByte();
        reader.ReadByte(); // defense id
        _defensePoints = reader.ReadByte();
        reader.ReadByte(); // crit rate id
        _critRate = reader.ReadByte();
        reader.ReadByte(); // crit damage id
        _critDamage = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        ElementalSpirit? spirit = player.getElementalSpirit(_type);
        bool result = false;
        if (spirit != null)
        {
            if (_attackPoints > 0 && spirit.getAvailableCharacteristicsPoints() >= _attackPoints)
            {
                spirit.addAttackPoints(_attackPoints);
                result = true;
            }

            if (_defensePoints > 0 && spirit.getAvailableCharacteristicsPoints() >= _defensePoints)
            {
                spirit.addDefensePoints(_defensePoints);
                result = true;
            }

            if (_critRate > 0 && spirit.getAvailableCharacteristicsPoints() >= _critRate)
            {
                spirit.addCritRatePoints(_critRate);
                result = true;
            }

            if (_critDamage > 0 && spirit.getAvailableCharacteristicsPoints() >= _critDamage)
            {
                spirit.addCritDamage(_critDamage);
                result = true;
            }
        }

        if (result)
        {
            if (!player.isSubclassLocked())
            {
                UserInfoPacket userInfo = new UserInfoPacket(player, false);
                userInfo.AddComponentType(UserInfoType.ATT_SPIRITS);
                connection.Send(userInfo);
            }

            connection.Send(SystemMessageId.CHARACTERISTICS_WERE_APPLIED_SUCCESSFULLY);
        }

        connection.Send(new ElementalSpiritSetTalentPacket(player, _type, result));

        return ValueTask.CompletedTask;
    }
}