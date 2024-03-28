using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExCubeGameRequestReadyPacket: IOutgoingPacket
{
    public static readonly ExCubeGameRequestReadyPacket STATIC_PACKET = new();
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BLOCK_UP_SET_LIST);
        
        writer.WriteInt32(4);
    }
}