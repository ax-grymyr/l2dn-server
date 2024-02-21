using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;

public readonly struct ElementalSpiritInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly byte _type;
	
    public ElementalSpiritInfoPacket(Player player, byte packetType)
    {
        _player = player;
        _type = packetType;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ELEMENTAL_SPIRIT_INFO);
        
        ElementalSpirit[] spirits = _player.getSpirits();
        if (spirits == null)
        {
            writer.WriteByte(0);
            writer.WriteByte(0);
            writer.WriteByte(0);
            return;
        }
		
        writer.WriteByte(_type); // show spirit info window 1; Change type 2; Only update 0
        writer.WriteByte((byte)spirits.Length); // spirit count
		
        foreach (ElementalSpirit spirit in spirits)
        {
            writer.WriteByte((byte)spirit.getType());
            writer.WriteByte(1);
            SpiritPacketHelper.WriteSpiritInfo(writer, spirit);
        }
		
        writer.WriteInt32(1); // Reset talent items count
        for (int i = 0; i < 1; i++)
        {
            writer.WriteInt32(57);
            writer.WriteInt64(50000);
        }
    }
}