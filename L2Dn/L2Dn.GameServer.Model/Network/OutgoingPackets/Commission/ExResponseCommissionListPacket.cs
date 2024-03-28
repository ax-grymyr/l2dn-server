using L2Dn.Extensions;
using L2Dn.GameServer.Model.Commission;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Commission;

public readonly struct ExResponseCommissionListPacket: IOutgoingPacket
{
	public const int MAX_CHUNK_SIZE = 120;
	
	private readonly CommissionListReplyType _replyType;
	private readonly List<CommissionItem>? _items;
	private readonly int _chunkId;
	private readonly int _listIndexStart;

	public ExResponseCommissionListPacket(CommissionListReplyType replyType)
	{
		_replyType = replyType;
	}

	public ExResponseCommissionListPacket(CommissionListReplyType replyType, List<CommissionItem>? items)
		: this(replyType, items, 0)
	{
	}

	public ExResponseCommissionListPacket(CommissionListReplyType replyType, List<CommissionItem> items, int chunkId)
		: this(replyType, items, chunkId, 0)
	{
	}

	public ExResponseCommissionListPacket(CommissionListReplyType replyType, List<CommissionItem> items, int chunkId, int listIndexStart)
	{
		_replyType = replyType;
		_items = items;
		_chunkId = chunkId;
		_listIndexStart = listIndexStart;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_RESPONSE_COMMISSION_LIST);
		writer.WriteInt32((int)_replyType);
		switch (_replyType)
		{
			case CommissionListReplyType.PLAYER_AUCTIONS:
			case CommissionListReplyType.AUCTIONS:
			{
				writer.WriteInt32(DateTime.UtcNow.getEpochSecond());
				writer.WriteInt32(_chunkId);
				int chunkSize = _items.Count - _listIndexStart;
				if (chunkSize > MAX_CHUNK_SIZE)
					chunkSize = MAX_CHUNK_SIZE;

				writer.WriteInt32(chunkSize);
				for (int i = _listIndexStart; i < (_listIndexStart + chunkSize); i++)
				{
					CommissionItem commissionItem = _items[i];
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