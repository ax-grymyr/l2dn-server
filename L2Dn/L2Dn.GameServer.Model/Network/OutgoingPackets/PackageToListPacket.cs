using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PackageToListPacket: IOutgoingPacket
{
    private readonly Map<int, string> _players;
	
    public PackageToListPacket(Map<int, string> chars)
    {
        _players = chars;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PACKAGE_TO_LIST);
        
        writer.WriteInt32(_players.size());
        foreach (var entry in _players)
        {
            writer.WriteInt32(entry.Key);
            writer.WriteString(entry.Value);
        }
    }
}