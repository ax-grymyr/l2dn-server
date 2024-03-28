using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.EquipmentUpgradeNormal;

public readonly struct ExShowUpgradeSystemNormalPacket: IOutgoingPacket
{
    private readonly int _mode;
    private readonly int _type;
    private readonly int _commission;
    private readonly List<int> _materials;
    private readonly List<int> _discountRatio;
	
    public ExShowUpgradeSystemNormalPacket(int mode, int type)
    {
        _mode = mode;
        _type = type;
        _commission = EquipmentUpgradeNormalData.getInstance().getCommission();
        _materials = new List<int>();
        _discountRatio = new List<int>();
        foreach (ItemHolder item in EquipmentUpgradeNormalData.getInstance().getDiscount())
        {
            _materials.Add(item.getId());
            _discountRatio.Add((int) item.getCount());
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_UPGRADE_SYSTEM_NORMAL);
        
        writer.WriteInt16((short)_mode);
        writer.WriteInt16((short)_type);
        writer.WriteInt16((short)_commission); // default - 100
        writer.WriteInt32(_materials.Count); // array of materials with discount
        foreach (int id in _materials)
        {
            writer.WriteInt32(id);
        }
        writer.WriteInt32(_discountRatio.Count); // array of discount count
        foreach (int discount in _discountRatio)
        {
            writer.WriteInt32(discount);
        }
    }
}