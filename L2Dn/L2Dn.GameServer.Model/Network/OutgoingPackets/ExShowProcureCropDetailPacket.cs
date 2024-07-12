using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowProcureCropDetailPacket: IOutgoingPacket
{
    private readonly int _cropId;
    private readonly Map<int, CropProcure> _castleCrops;
	
    public ExShowProcureCropDetailPacket(int cropId)
    {
        _cropId = cropId;
        _castleCrops = new Map<int, CropProcure>();
        foreach (Castle c in CastleManager.getInstance().getCastles())
        {
            CropProcure cropItem = CastleManorManager.getInstance().getCropProcure(c.getResidenceId(), cropId, false);
            if ((cropItem != null) && (cropItem.getAmount() > 0))
            {
                _castleCrops.put(c.getResidenceId(), cropItem);
            }
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_PROCURE_CROP_DETAIL);
        
        writer.WriteInt32(_cropId); // crop id
        writer.WriteInt32(_castleCrops.Count); // size
        foreach (var entry in _castleCrops)
        {
            CropProcure crop = entry.Value;
            writer.WriteInt32(entry.Key); // manor name
            writer.WriteInt64(crop.getAmount()); // buy residual
            writer.WriteInt64(crop.getPrice()); // buy price
            writer.WriteByte((byte)crop.getReward()); // reward type
        }
    }
}