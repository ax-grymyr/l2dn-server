using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Surveillance;

public readonly struct ExUserWatcherTargetStatusPacket: IOutgoingPacket
{
    private readonly string _name;
    private readonly bool _online;
	
    public ExUserWatcherTargetStatusPacket(string name, bool online)
    {
        _name = name;
        _online = online;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_USER_WATCHER_TARGET_STATUS);
        
        writer.WriteSizedString(_name);
        writer.WriteInt32(0); // client.getProxyServerId()
        writer.WriteByte(_online);
    }
}