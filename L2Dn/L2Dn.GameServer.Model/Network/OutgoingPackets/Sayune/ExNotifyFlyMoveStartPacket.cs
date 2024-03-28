using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Sayune;

public readonly struct ExNotifyFlyMoveStartPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
		writer.WritePacketCode(OutgoingPacketCodes.EX_NOTIFY_FLY_MOVE_START);
    }
}