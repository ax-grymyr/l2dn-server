using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.ElementalSpirits;

public readonly struct ElementalSpiritEvolutionInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly ElementalType _type;
	
    public ElementalSpiritEvolutionInfoPacket(Player player, ElementalType type)
    {
        _player = player;
        _type = type;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ELEMENTAL_SPIRIT_EVOLUTION_INFO);
        
        ElementalSpirit spirit = _player.getElementalSpirit(_type);
        if (spirit == null)
        {
            writer.WriteByte(0);
            writer.WriteInt32(0);
            return;
        }
        
        writer.WriteByte((byte)_type);
        writer.WriteInt32(spirit.getNpcId());
        writer.WriteInt32(1); // unk
        writer.WriteInt32(spirit.getStage());
        writer.WriteDouble(100); // chance ??
        List<ItemHolder> items = spirit.getItemsToEvolve();
        writer.WriteInt32(items.Count);
        foreach (ItemHolder item in items)
        {
            writer.WriteInt32(item.getId());
            writer.WriteInt64(item.getCount());
        }
    }
}