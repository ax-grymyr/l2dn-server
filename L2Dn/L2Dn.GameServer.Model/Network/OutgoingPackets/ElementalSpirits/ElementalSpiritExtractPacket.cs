using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;

public readonly struct ElementalSpiritExtractPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly ElementalType _type;
    private readonly bool _extracted;

    public ElementalSpiritExtractPacket(Player player, ElementalType type, bool extracted)
    {
        _player = player;
        _type = type;
        _extracted = extracted;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ELEMENTAL_SPIRIT_EXTRACT);
        
        ElementalPacketHelper.WriteUpdate(writer, _player, _type, _extracted);
    }
}