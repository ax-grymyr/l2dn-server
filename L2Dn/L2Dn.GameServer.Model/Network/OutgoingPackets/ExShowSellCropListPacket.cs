using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowSellCropListPacket: IOutgoingPacket
{
    private readonly int _manorId;
    private readonly Map<int, Item> _cropsItems;
    private readonly Map<int, CropProcure> _castleCrops;
	
    public ExShowSellCropListPacket(PlayerInventory inventory, int manorId)
    {
        _manorId = manorId;
        _castleCrops = new Map<int, CropProcure>();
        _cropsItems = new Map<int, Item>();
        foreach (int cropId in CastleManorManager.getInstance().getCropIds())
        {
            Item item = inventory.getItemByItemId(cropId);
            if (item != null)
            {
                _cropsItems.put(cropId, item);
            }
        }
        foreach (CropProcure crop in CastleManorManager.getInstance().getCropProcure(_manorId, false))
        {
            if (_cropsItems.ContainsKey(crop.getId()) && (crop.getAmount() > 0))
            {
                _castleCrops.put(crop.getId(), crop);
            }
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_SELL_CROP_LIST);
        
        writer.WriteInt32(_manorId); // manor id
        writer.WriteInt32(_cropsItems.Count); // size
        foreach (Item item in _cropsItems.Values)
        {
            Seed seed = CastleManorManager.getInstance().getSeedByCrop(item.getId());
            writer.WriteInt32(item.getObjectId()); // Object id
            writer.WriteInt32(item.getId()); // crop id
            writer.WriteInt32(seed.getLevel()); // seed level
            writer.WriteByte(1);
            writer.WriteInt32(seed.getReward(1)); // reward 1 id
            writer.WriteByte(1);
            writer.WriteInt32(seed.getReward(2)); // reward 2 id

            if (_castleCrops.TryGetValue(item.getId(), out CropProcure? crop))
            {
                writer.WriteInt32(_manorId); // manor
                writer.WriteInt64(crop.getAmount()); // buy residual
                writer.WriteInt64(crop.getPrice()); // buy price
                writer.WriteByte((byte)crop.getReward()); // reward
            }
            else
            {
                writer.WriteInt32(-1); // manor
                writer.WriteInt64(0); // buy residual
                writer.WriteInt64(0); // buy price
                writer.WriteByte(0); // reward
            }
            
            writer.WriteInt64(item.getCount()); // my crops
        }
    }
}