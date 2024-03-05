using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.NewHenna;

public readonly struct NewHennaPotenEnchantPacket: IOutgoingPacket
{
    private readonly int _slotId;
    private readonly int _enchantStep;
    private readonly int _enchantExp;
    private readonly int _dailyStep;
    private readonly int _dailyCount;
    private readonly int _activeStep;
    private readonly bool _success;
	
    public NewHennaPotenEnchantPacket(int slotId, int enchantStep, int enchantExp, int dailyStep, int dailyCount, int activeStep, bool success)
    {
        _slotId = slotId;
        _enchantStep = enchantStep;
        _enchantExp = enchantExp;
        _dailyStep = dailyStep;
        _dailyCount = dailyCount;
        _activeStep = activeStep;
        _success = success;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_NEW_HENNA_POTEN_ENCHANT);
        
        writer.WriteByte((byte)_slotId);
        writer.WriteInt16((short)_enchantStep);
        writer.WriteInt32(_enchantExp);
        writer.WriteInt16((short)_dailyStep);
        writer.WriteInt16((short)_dailyCount);
        writer.WriteInt16((short)_activeStep);
        writer.WriteByte(_success);
        writer.WriteInt16((short)_dailyStep);
        writer.WriteInt16((short)_dailyCount);
    }
}