using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExChangeNpcStatePacket: IOutgoingPacket
{
    private readonly int _objId;
    private readonly int _state;
	
    public ExChangeNpcStatePacket(int objId, int state)
    {
        _objId = objId;
        _state = state;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CHANGE_NPC_STATE);
        writer.WriteInt32(_objId);
        writer.WriteInt32(_state);
    }
}