using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowTerritoryPacket: IOutgoingPacket
{
    private readonly int _minZ;
    private readonly int _maxZ;
    private readonly List<ILocational> _vertices;
	
    public ExShowTerritoryPacket(int minZ, int maxZ)
    {
        _minZ = minZ;
        _maxZ = maxZ;
        _vertices = new List<ILocational>();
    }
	
    public void addVertice(ILocational loc)
    {
        _vertices.add(loc);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_TERRITORY);
        
        writer.WriteInt32(_vertices.size());
        writer.WriteInt32(_minZ);
        writer.WriteInt32(_maxZ);
        foreach (ILocational loc in _vertices)
        {
            writer.WriteInt32(loc.getX());
            writer.WriteInt32(loc.getY());
        }
    }
}