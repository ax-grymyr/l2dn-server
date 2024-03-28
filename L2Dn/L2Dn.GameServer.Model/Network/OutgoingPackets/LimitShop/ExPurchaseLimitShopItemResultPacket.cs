using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.LimitShop;

public readonly struct ExPurchaseLimitShopItemResultPacket: IOutgoingPacket
{
    private readonly int _category, _productId;
    private readonly bool _isSuccess;
    private readonly int _remainingInfo;
    private readonly ICollection<LimitShopRandomCraftReward> _rewards;
	
    public ExPurchaseLimitShopItemResultPacket(bool isSuccess, int category, int productId, int remainingInfo, ICollection<LimitShopRandomCraftReward> rewards)
    {
        _isSuccess = isSuccess;
        _category = category;
        _productId = productId;
        _remainingInfo = remainingInfo;
        _rewards = rewards;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PURCHASE_LIMIT_SHOP_ITEM_BUY);
        
        writer.WriteByte(!_isSuccess);
        writer.WriteByte((byte)_category);
        writer.WriteInt32(_productId);
        writer.WriteInt32(_rewards.Count);
        foreach (LimitShopRandomCraftReward entry in _rewards)
        {
            writer.WriteByte((byte)entry.getRewardIndex());
            writer.WriteInt32(entry.getItemId());
            writer.WriteInt32(entry.Count);
        }
        
        writer.WriteInt32(_remainingInfo);
    }
}