using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.ItemAuction;

public class ItemAuctionBid
{
    private readonly int _playerObjId;
    private long _lastBid;

    public ItemAuctionBid(int playerObjId, long lastBid)
    {
        _playerObjId = playerObjId;
        _lastBid = lastBid;
    }

    public int getPlayerObjId()
    {
        return _playerObjId;
    }

    public long getLastBid()
    {
        return _lastBid;
    }

    public void setLastBid(long lastBid)
    {
        _lastBid = lastBid;
    }

    public void cancelBid()
    {
        _lastBid = -1;
    }

    public bool isCanceled()
    {
        return _lastBid <= 0;
    }

    public Player getPlayer()
    {
        return World.getInstance().getPlayer(_playerObjId);
    }
}