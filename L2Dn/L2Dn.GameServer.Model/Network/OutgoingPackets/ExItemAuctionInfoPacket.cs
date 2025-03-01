using L2Dn.Extensions;
using L2Dn.GameServer.Model.ItemAuction;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExItemAuctionInfoPacket: IOutgoingPacket
{
    private readonly bool _refresh;
    private readonly int _timeRemaining;
    private readonly ItemAuction _currentAuction;
    private readonly ItemAuction? _nextAuction;

    public ExItemAuctionInfoPacket(bool refresh, ItemAuction currentAuction, ItemAuction? nextAuction)
    {
        if (currentAuction.getAuctionState() != ItemAuctionState.STARTED)
        {
            _timeRemaining = 0;
        }
        else
        {
            _timeRemaining = (int)currentAuction.getFinishingTimeRemaining().TotalSeconds;
        }

        _refresh = refresh;
        _currentAuction = currentAuction;
        _nextAuction = nextAuction;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ITEM_AUCTION_INFO);

        writer.WriteByte(!_refresh);
        writer.WriteInt32(_currentAuction.getInstanceId());
        ItemAuctionBid? highestBid = _currentAuction.getHighestBid();
        writer.WriteInt64(highestBid != null ? highestBid.getLastBid() : _currentAuction.getAuctionInitBid());
        writer.WriteInt32(_timeRemaining);
        InventoryPacketHelper.WriteItem(writer, _currentAuction.getItemInfo());
        if (_nextAuction != null)
        {
            writer.WriteInt64(_nextAuction.getAuctionInitBid());
            writer.WriteInt32(_nextAuction.getStartingTime().getEpochSecond()); // unix time in seconds
            InventoryPacketHelper.WriteItem(writer, _nextAuction.getItemInfo());
        }
    }
}