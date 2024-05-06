using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;

public readonly struct ElementalSpiritEvolutionPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly ElementalType _type;
    private readonly bool _evolved;

    public ElementalSpiritEvolutionPacket(Player player, ElementalType type, bool evolved)
    {
        _player = player;
        _type = type;
        _evolved = evolved;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ELEMENTAL_SPIRIT_EVOLUTION);
        
        ElementalPacketHelper.WriteUpdate(writer, _player, _type, _evolved);
    }
}