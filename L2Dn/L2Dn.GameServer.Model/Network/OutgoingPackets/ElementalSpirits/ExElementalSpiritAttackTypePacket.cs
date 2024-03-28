using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;

public readonly struct ExElementalSpiritAttackTypePacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public ExElementalSpiritAttackTypePacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ELEMENTAL_SPIRIT_ATTACK_TYPE);
        
        ElementalType elementalId = _player.getActiveElementalSpiritType();
        if (elementalId == ElementalType.WIND)
        {
            writer.WriteByte(4);
        }
        else if (elementalId == ElementalType.EARTH)
        {
            writer.WriteByte(8);
        }
        else
        {
            writer.WriteByte((byte)elementalId);
        }
    }
}