using L2Dn.GameServer.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SnoopPacket: IOutgoingPacket
{
    private readonly int _convoId;
    private readonly string _name;
    private readonly ChatType _type;
    private readonly string _speaker;
    private readonly string _msg;
	
    public SnoopPacket(int id, string name, ChatType type, string speaker, string msg)
    {
        _convoId = id;
        _name = name;
        _type = type;
        _speaker = speaker;
        _msg = msg;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SNOOP);
        
        writer.WriteInt32(_convoId);
        writer.WriteString(_name);
        writer.WriteInt32(0); // ??
        writer.WriteInt32((int)_type);
        writer.WriteString(_speaker);
        writer.WriteString(_msg);
    }
}