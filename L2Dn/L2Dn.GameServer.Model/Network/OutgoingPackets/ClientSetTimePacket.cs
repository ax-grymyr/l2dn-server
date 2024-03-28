using L2Dn.GameServer.TaskManagers;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ClientSetTimePacket: IOutgoingPacket
{
    public static readonly ClientSetTimePacket STATIC_PACKET = new();
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.CLIENT_SET_TIME);
        
        writer.WriteInt32(GameTimeTaskManager.getInstance().getGameTime()); // Time in client minutes.
        writer.WriteInt32(GameTimeTaskManager.IG_DAYS_PER_DAY); // Constant to match the server time. This determines the speed of the client clock.
    }
}