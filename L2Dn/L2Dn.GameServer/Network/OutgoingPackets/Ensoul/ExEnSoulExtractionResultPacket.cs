using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ensoul;

public readonly struct ExEnSoulExtractionResultPacket: IOutgoingPacket
{
    private readonly bool _success;
    private readonly Item _item;
	
    public ExEnSoulExtractionResultPacket(bool success, Item item)
    {
        _success = success;
        _item = item;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENSOUL_EXTRACTION_RESULT);
        
        writer.WriteByte(_success);
        if (_success)
        {
            writer.WriteByte((byte)_item.getSpecialAbilities().Count);
            foreach (EnsoulOption option in _item.getSpecialAbilities())
            {
                writer.WriteInt32(option.getId());
            }
            
            writer.WriteByte((byte)_item.getAdditionalSpecialAbilities().Count);
            foreach (EnsoulOption option in _item.getAdditionalSpecialAbilities())
            {
                writer.WriteInt32(option.getId());
            }
        }
    }
}