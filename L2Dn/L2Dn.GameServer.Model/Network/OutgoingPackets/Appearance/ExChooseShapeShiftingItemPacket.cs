using L2Dn.GameServer.Model.Items.Appearance;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Appearance;

public readonly struct ExChooseShapeShiftingItemPacket: IOutgoingPacket
{
    private readonly AppearanceType _type;
    private readonly AppearanceTargetType _targetType;
    private readonly int _itemId;
	
    public ExChooseShapeShiftingItemPacket(AppearanceStone stone)
    {
        _type = stone.getType();
        _targetType = stone.getTargetTypes().size() > 1 ? AppearanceTargetType.ALL : stone.getTargetTypes().First();
        _itemId = stone.getId();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CHOOSE_SHAPE_SHIFTING_ITEM);
        
        writer.WriteInt32((int)_targetType);
        writer.WriteInt32((int)_type);
        writer.WriteInt32(_itemId);
    }
}