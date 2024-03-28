using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;

public readonly struct ElementalSpiritAbsorbPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly ElementalType _type;
    private readonly bool _absorbed;

    public ElementalSpiritAbsorbPacket(Player player, ElementalType type, bool absorbed)
    {
        _player = player;
        _type = type;
        _absorbed = absorbed;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ELEMENTAL_SPIRIT_ABSORB);
        ElementalPacketHelper.WriteUpdate(writer, _player, _type, _absorbed);
    }
}