using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ShowTownMapPacket: IOutgoingPacket
{
    private readonly string _texture;
    private readonly int _x;
    private readonly int _y;
	
    public ShowTownMapPacket(string texture, int x, int y)
    {
        _texture = texture;
        _x = x;
        _y = y;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SHOW_TOWN_MAP);
        writer.WriteString(_texture);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
    }
}