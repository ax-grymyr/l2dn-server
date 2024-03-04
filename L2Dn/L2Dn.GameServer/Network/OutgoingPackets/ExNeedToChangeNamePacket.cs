using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExNeedToChangeNamePacket: IOutgoingPacket
{
    private readonly int _type;
    private readonly int _subType;
    private readonly String _name;
	
    public ExNeedToChangeNamePacket(int type, int subType, String name)
    {
        _type = type;
        _subType = subType;
        _name = name;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_NEED_TO_CHANGE_NAME);
        
        writer.WriteInt32(_type);
        writer.WriteInt32(_subType);
        writer.WriteString(_name);
    }
}