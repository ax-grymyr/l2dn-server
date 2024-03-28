using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowCropSettingPacket: IOutgoingPacket
{
    private readonly int _manorId;
    private readonly Set<Seed> _seeds;
    private readonly Map<int, CropProcure> _current;
    private readonly Map<int, CropProcure> _next;
	
    public ExShowCropSettingPacket(int manorId)
    {
        CastleManorManager manor = CastleManorManager.getInstance();
        _manorId = manorId;
        _seeds = manor.getSeedsForCastle(_manorId);
        _current = new Map<int, CropProcure>();
        _next = new Map<int, CropProcure>();
        foreach (Seed s in _seeds)
        {
            // Current period
            CropProcure cp = manor.getCropProcure(manorId, s.getCropId(), false);
            if (cp != null)
            {
                _current.put(s.getCropId(), cp);
            }
            // Next period
            cp = manor.getCropProcure(manorId, s.getCropId(), true);
            if (cp != null)
            {
                _next.put(s.getCropId(), cp);
            }
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_CROP_SETTING);
        
        writer.WriteInt32(_manorId); // manor id
        writer.WriteInt32(_seeds.size()); // size
        foreach (Seed s in _seeds)
        {
            writer.WriteInt32(s.getCropId()); // crop id
            writer.WriteInt32(s.getLevel()); // seed level
            writer.WriteByte(1);
            writer.WriteInt32(s.getReward(1)); // reward 1 id
            writer.WriteByte(1);
            writer.WriteInt32(s.getReward(2)); // reward 2 id
            writer.WriteInt32(s.getCropLimit()); // next sale limit
            writer.WriteInt32(0); // ???
            writer.WriteInt32(s.getCropMinPrice()); // min crop price
            writer.WriteInt32(s.getCropMaxPrice()); // max crop price
            // Current period
            if (_current.containsKey(s.getCropId()))
            {
                CropProcure cp = _current.get(s.getCropId());
                writer.WriteInt64(cp.getStartAmount()); // buy
                writer.WriteInt64(cp.getPrice()); // price
                writer.WriteByte((byte)cp.getReward()); // reward
            }
            else
            {
                writer.WriteInt64(0);
                writer.WriteInt64(0);
                writer.WriteByte(0);
            }
            // Next period
            if (_next.containsKey(s.getCropId()))
            {
                CropProcure cp = _next.get(s.getCropId());
                writer.WriteInt64(cp.getStartAmount()); // buy
                writer.WriteInt64(cp.getPrice()); // price
                writer.WriteByte((byte)cp.getReward()); // reward
            }
            else
            {
                writer.WriteInt64(0);
                writer.WriteInt64(0);
                writer.WriteByte(0);
            }
        }
        
        _next.clear();
        _current.clear();
    }
}