using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowManorDefaultInfoPacket: IOutgoingPacket
{
    private readonly List<Seed> _crops;
    private readonly bool _hideButtons;
	
    public ExShowManorDefaultInfoPacket(bool hideButtons)
    {
        _crops = CastleManorManager.getInstance().getCrops();
        _hideButtons = hideButtons;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_MANOR_DEFAULT_INFO);
        
        writer.WriteByte(_hideButtons); // Hide "Seed Purchase" and "Crop Sales" buttons
        writer.WriteInt32(_crops.Count);
        foreach (Seed crop in _crops)
        {
            writer.WriteInt32(crop.getCropId()); // crop Id
            writer.WriteInt32(crop.getLevel()); // level
            writer.WriteInt32(crop.getSeedReferencePrice()); // seed price
            writer.WriteInt32(crop.getCropReferencePrice()); // crop price
            writer.WriteByte(1); // Reward 1 type
            writer.WriteInt32(crop.getReward(1)); // Reward 1 itemId
            writer.WriteByte(1); // Reward 2 type
            writer.WriteInt32(crop.getReward(2)); // Reward 2 itemId
        }
    }
}