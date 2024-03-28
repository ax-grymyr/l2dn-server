using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPledgeWaitingListAlarmPacket: IOutgoingPacket
{
    public static readonly ExPledgeWaitingListAlarmPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_WAITING_LIST_ALARM);
    }
}