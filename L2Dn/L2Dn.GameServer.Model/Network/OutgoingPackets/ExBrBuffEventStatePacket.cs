using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExBrBuffEventStatePacket: IOutgoingPacket
{
    private readonly int _type; // 1 - %, 2 - npcId
    private readonly int _value; // depending on type: for type 1 - % value; for type 2 - 20573-20575
    private readonly int _state; // 0-1
    private readonly int _endtime; // only when type 2 as unix time in seconds from 1970
	
    public ExBrBuffEventStatePacket(int type, int value, int state, int endtime)
    {
        _type = type;
        _value = value;
        _state = state;
        _endtime = endtime;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BR_BUFF_EVENT_STATE);

        writer.WriteInt32(_type);
        writer.WriteInt32(_value);
        writer.WriteInt32(_state);
        writer.WriteInt32(_endtime);
    }
}