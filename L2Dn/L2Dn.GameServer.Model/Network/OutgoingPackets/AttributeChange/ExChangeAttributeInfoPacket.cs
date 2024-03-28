using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.AttributeChange;

public readonly struct ExChangeAttributeInfoPacket: IOutgoingPacket
{
    private readonly int _crystalItemId;
    private readonly int _attributes;
    private readonly int _itemObjId;
	
    public ExChangeAttributeInfoPacket(int crystalItemId, Item item)
    {
        _crystalItemId = crystalItemId;
        _attributes = 0;
        foreach (AttributeType e in AttributeTypeUtil.AttributeTypes)
        {
            if (e != item.getAttackAttributeType())
            {
                _attributes |= 1 << (int)e;
            }
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CHANGE_ATTRIBUTE_INFO);
        
        writer.WriteInt32(_crystalItemId);
        writer.WriteInt32(_attributes);
        writer.WriteInt32(_itemObjId);
    }
}