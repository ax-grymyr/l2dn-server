using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.NewHenna;

public readonly struct NewHennaPotenSelectPacket: IOutgoingPacket
{
    private readonly int _slotId;
    private readonly int _potenId;
    private readonly int _activeStep;
    private readonly bool _success;
	
    public NewHennaPotenSelectPacket(int slotId, int potenId, int activeStep, bool success)
    {
        _slotId = slotId;
        _potenId = potenId;
        _activeStep = activeStep;
        _success = success;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_NEW_HENNA_POTEN_SELECT);
        
        writer.WriteByte((byte)_slotId);
        writer.WriteInt32(_potenId);
        writer.WriteInt16((short)_activeStep);
        writer.WriteByte(_success);
    }
}