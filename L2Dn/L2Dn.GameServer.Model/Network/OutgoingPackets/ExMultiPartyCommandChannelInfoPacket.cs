using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExMultiPartyCommandChannelInfoPacket: IOutgoingPacket
{
    private readonly CommandChannel _channel;
	
    public ExMultiPartyCommandChannelInfoPacket(CommandChannel channel)
    {
        _channel = channel;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MULTI_PARTY_COMMAND_CHANNEL_INFO);
        
        writer.WriteString(_channel.getLeader().getName());
        writer.WriteInt32(0); // Channel loot 0 or 1
        writer.WriteInt32(_channel.getMemberCount());
        writer.WriteInt32(_channel.getParties().Count);
        foreach (Party p in _channel.getParties())
        {
            writer.WriteString(p.getLeader().getName());
            writer.WriteInt32(p.getLeaderObjectId());
            writer.WriteInt32(p.getMemberCount());
        }
    }
}