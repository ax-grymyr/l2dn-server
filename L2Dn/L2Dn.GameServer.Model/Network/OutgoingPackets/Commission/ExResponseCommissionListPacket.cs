using L2Dn.Extensions;
using L2Dn.GameServer.Model.Commission;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Commission;

public readonly struct ExResponseCommissionListPacket(
    CommissionListReplyType replyType, List<CommissionItem>? items = null, int chunkId = 0, int listIndexStart = 0)
    : IOutgoingPacket
{
    public const int MAX_CHUNK_SIZE = 120;

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RESPONSE_COMMISSION_LIST);
        writer.WriteInt32((int)replyType);
        switch (replyType)
        {
            case CommissionListReplyType.PLAYER_AUCTIONS:
            case CommissionListReplyType.AUCTIONS:
            {
                writer.WriteInt32(DateTime.UtcNow.getEpochSecond());
                writer.WriteInt32(chunkId);
                int chunkSize = items!.Count - listIndexStart; // TODO: !!!
                if (chunkSize > MAX_CHUNK_SIZE)
                    chunkSize = MAX_CHUNK_SIZE;

                writer.WriteInt32(chunkSize);
                for (int i = listIndexStart; i < listIndexStart + chunkSize; i++)
                {
                    CommissionItem commissionItem = items[i];
                    writer.WriteInt64(commissionItem.getCommissionId());
                    writer.WriteInt64(commissionItem.getPricePerUnit());
                    writer.WriteInt32(0); // CommissionItemType seems client does not really need it.
                    writer.WriteInt32((commissionItem.getDurationInDays() - 1) / 2);
                    writer.WriteInt32(commissionItem.getEndTime().getEpochSecond());
                    writer.WriteString(string.Empty); // Seller Name is not displayed anywhere so i am not sending it to decrease traffic.
                    InventoryPacketHelper.WriteItem(writer, commissionItem.getItemInfo());
                }

                break;
            }
        }
    }
}