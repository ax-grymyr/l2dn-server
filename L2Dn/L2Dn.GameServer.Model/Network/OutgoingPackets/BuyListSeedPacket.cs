using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct BuyListSeedPacket: IOutgoingPacket
{
    private readonly int _manorId;
    private readonly long _money;
    private readonly List<SeedProduction> _list;
	
    public BuyListSeedPacket(long currentMoney, int castleId)
    {
        _money = currentMoney;
        _manorId = castleId;
        _list = new List<SeedProduction>();
        foreach (SeedProduction s in CastleManorManager.getInstance().getSeedProduction(castleId, false))
        {
            if (s.getAmount() > 0 && s.getPrice() > 0)
            {
                _list.Add(s);
            }
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.BUY_LIST_SEED);
        
        writer.WriteInt64(_money); // current money
        writer.WriteInt32(0); // TODO: Find me!
        writer.WriteInt32(_manorId); // manor id
        if (_list.Count != 0)
        {
            writer.WriteInt16((short)_list.Count); // list length
            foreach (SeedProduction s in _list)
            {
                writer.WriteByte(0); // mask item 0 to print minimum item information
                writer.WriteInt32(s.getId()); // ObjectId
                writer.WriteInt32(s.getId()); // ItemId
                writer.WriteByte(0xFF); // T1
                writer.WriteInt64(s.getAmount()); // Quantity
                writer.WriteByte(5); // Item Type 2 : 00-weapon, 01-shield/armor, 02-ring/earring/necklace, 03-questitem, 04-adena, 05-item
                writer.WriteByte(0); // Filler (always 0)
                writer.WriteInt16(0); // Equipped : 00-No, 01-yes
                writer.WriteInt64(0); // Slot : 0006-lr.ear, 0008-neck, 0030-lr.finger, 0040-head, 0100-l.hand, 0200-gloves, 0400-chest, 0800-pants, 1000-feet, 4000-r.hand, 8000-r.hand
                writer.WriteInt16(0); // Enchant level (pet level shown in control item)
                writer.WriteInt32(-1);
                writer.WriteInt32(-9999);
                writer.WriteByte(1); // GOD Item enabled = 1 disabled (red) = 0
                writer.WriteInt64(s.getPrice()); // price
            }
        }
        else
        {
            writer.WriteInt16(0);
        }
    }
}