using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Teleports;

public readonly struct ExShowSharingLocationUiPacket: IOutgoingPacket
{
    public static readonly ExShowSharingLocationUiPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHARED_POSITION_SHARING_UI);
        
        writer.WriteInt64(Config.SHARING_LOCATION_COST);
    }
}