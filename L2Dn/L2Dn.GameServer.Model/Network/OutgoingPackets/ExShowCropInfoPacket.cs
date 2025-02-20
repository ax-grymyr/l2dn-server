using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowCropInfoPacket: IOutgoingPacket
{
    private readonly List<CropProcure>? _crops;
    private readonly int _manorId;
    private readonly bool _hideButtons;

    public ExShowCropInfoPacket(int manorId, bool nextPeriod, bool hideButtons)
    {
        _manorId = manorId;
        _hideButtons = hideButtons;
        CastleManorManager manor = CastleManorManager.getInstance();
        _crops = nextPeriod && !manor.isManorApproved() ? null : manor.getCropProcure(manorId, nextPeriod);
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_CROP_INFO);

        writer.WriteByte(_hideButtons); // Hide "Crop Sales" button
        writer.WriteInt32(_manorId); // Manor ID
        writer.WriteInt32(0);
        if (_crops != null)
        {
            writer.WriteInt32(_crops.Count);
            foreach (CropProcure crop in _crops)
            {
                writer.WriteInt32(crop.getId()); // Crop id
                writer.WriteInt64(crop.getAmount()); // Buy residual
                writer.WriteInt64(crop.getStartAmount()); // Buy
                writer.WriteInt64(crop.getPrice()); // Buy price
                writer.WriteByte((byte)crop.getReward()); // Reward
                Seed? seed = CastleManorManager.getInstance().getSeedByCrop(crop.getId());
                if (seed == null)
                {
                    writer.WriteInt32(0); // Seed level
                    writer.WriteByte(1); // Reward 1
                    writer.WriteInt32(0); // Reward 1 - item id
                    writer.WriteByte(1); // Reward 2
                    writer.WriteInt32(0); // Reward 2 - item id
                }
                else
                {
                    writer.WriteInt32(seed.getLevel()); // Seed level
                    writer.WriteByte(1); // Reward 1
                    writer.WriteInt32(seed.getReward(1)); // Reward 1 - item id
                    writer.WriteByte(1); // Reward 2
                    writer.WriteInt32(seed.getReward(2)); // Reward 2 - item id
                }
            }
        }
    }
}